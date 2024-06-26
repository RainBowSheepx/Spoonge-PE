﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using SpoongePE.Core.Network;
using SpoongePE.Core.RakNet;
using SpoongePE.Core.Utils;

namespace SpoongePE.Core.Game;

public class ServerWorld
{
    private readonly Dictionary<RakNetClient, Player> ConnectionMap = new();

    public List<Entity> Entities = new();
    public World World;
    private GameServer Server;
    public bool sendFullChunks = true;
    private int ticks;
    public double currentTPS; // need to check
    Stopwatch stopwatch = new Stopwatch();
    
    public ServerWorld(GameServer server, World world)
    {
        Server = server;
        World = world;
        ticks = 0;
        stopwatch.Start();

        // Activate World ticking
        // 1 tick is 50ms, but StartRepeatingTask take ~3 mills from task
        RakNetServer.StartRepeatingTask(doTick, TimeSpan.FromMilliseconds(46));
        RakNetServer.StartRepeatingTask(TimeSendUpdate, TimeSpan.FromMilliseconds(2000));
    }

    public IEnumerable<Player> Players => ConnectionMap.Values;

    public async Task doTick()
    {
        ticks++;
        if (stopwatch.Elapsed.TotalSeconds >= 1)
        {
            currentTPS = ticks;
            stopwatch.Restart();
            Console.Title = $"SpoongePE Core :) TPS: {currentTPS} | Players: {World.players.Count()}/{RakNetServer.Properties.maxPlayers} | WorldTime: {World.worldTime}";

            ticks = 0;
        }

        AxisAlignedBB.clearBoundingBoxPool();
        Vec3D.initialize();

        await World.tick();
        World.updateEntities();

        World.entityTracker.updateTrackedEntities();


    }

    public async Task TimeSendUpdate()
    {

        SendAll(new SetTimePacket
        {
            Time = (int)World.worldTime,
        }
        );
    }

    public void KickAll()
    {
        //Хуёво работает блять
        SendAll(new PlayerDisconnectPacket());
    }

    public void SendAll(ConnectedPacket packet, ulong except = 0)
    {
        foreach (var player in Players)
            if (player.PlayerID != except)
                player.Send(packet);

    }

    public Player GetByName(string name)
    {
        return Players.FirstOrDefault(x => x.Username.ToLower() == name.ToLower(), null);
    }

    // public RakNetConnection GetPlayerByName(string name) => Players.FirstOrDefault(P => P.Player?.Username == name);

    public Player AddPlayer(RakNetClient client, ulong clientId, string username)
    {
        var newPlayer = new Player(client, this.World)
        {
            PlayerID = clientId,
            Username = username,
            DisplayName = username
        };

        newPlayer.playerData = new NbtFile();
        string playersFolder = Path.Combine("worlds", World.LevelName, "players");
        if (!Directory.Exists(playersFolder))
            Directory.CreateDirectory(playersFolder);
        else
           if (File.Exists(Path.Combine(playersFolder, $"{newPlayer.Username}.dat")))
            newPlayer.playerData.LoadFromFile(Path.Combine(playersFolder, $"{newPlayer.Username}.dat"));

        newPlayer.LoadDat();


        SendAll(new AddPlayerPacket
        {
            PlayerId = newPlayer.PlayerID,
            Username = newPlayer.Username,
            EntityId = newPlayer.EntityID,
            Pos = newPlayer.Position
        }
        );





        World.addPlayer(newPlayer);
        ConnectionMap.Add(client, newPlayer);
        client.player = newPlayer;
        return ConnectionMap[client];
    }

    public void RemovePlayer(RakNetClient client, string reason)
    {
        if (!ConnectionMap.TryGetValue(client, out var disconnectingPlayer))
            return;

        client.player.SaveDat();

        ConnectionMap.Remove(client);
        World.removePlayer(disconnectingPlayer.EntityID);
        SendAll(new RemovePlayerPacket
        {
            EntityId = disconnectingPlayer.EntityID,
            PlayerId = disconnectingPlayer.PlayerID
        }
        );

        SendAll(new ChatPacket
        {
            Message = $"{disconnectingPlayer.Username} left the game. ({reason})"
        }
        );
    }

    public void MovePlayer(RakNetClient client, Vector3 position, Vector3 viewAngle)
    {
        if (!ConnectionMap.TryGetValue(client, out var movingPlayer))
            return;

        movingPlayer.Position = position;
        movingPlayer.posX = position.X;
        movingPlayer.posY = position.Y;
        movingPlayer.posZ = position.Z;
        movingPlayer.ViewAngle = viewAngle;

        SendAll(new MovePlayerPacket
        {
            EntityId = movingPlayer.EntityID,
            Pos = movingPlayer.Position,
            Rot = movingPlayer.ViewAngle
        }, movingPlayer.PlayerID
        );
    }
    public void SendChunk(RakNetClient client, int X, int Z)
    {
        if (!ConnectionMap.TryGetValue(client, out var movingPlayer))
        {
            Logger.Warn("Can't find player for chunk send!");
            return;
        }

        if (X > 15 || X < 0 || Z > 15 || Z < 0)
        {
            Logger.Warn("Player " + client.ClientID + " tried to get invalid chunk! (" + X + " ," + Z + ")");
            return;
        }
        ChunkDataPacket cdp = new ChunkDataPacket();
        cdp.X = X;
        cdp.Z = Z;
        byte[] cd = new byte[16 * 16 * 128 + 16 * 16 * 64 + 16 * 16];
        int l = 0;
        Chunk c = World._chunks[X, Z];

        for (int z = 0; z < 16; ++z)
        {
            for (int x = 0; x < 16; ++x)
            {
                cd[l++] = (byte)0xff;

                for (int y = 0; y < 8; ++y)
                {
                    for (int yB = 0; yB < 16; ++yB)
                    {
                        byte id = c.BlockData[x, z, y * 16 + yB];
                        cd[l++] = id;
                    }

                    for (int yB = 0; yB < 8; ++yB)
                    {
                        byte meta = (byte)(c.BlockMetadata[x, z, y * 16 + yB] + (c.BlockMetadata[x, z, y * 16 + yB + 1] << 4));
                        cd[l++] = meta;
                    }
                }
            }
        }
        cdp.ChunkData = cd;

        client.Send(cdp);

        //  SendAll(new MessagePacket { Username = "Test", Message = "Test1" }) ;
    }
    public void SendChunkFromRequest(RakNetClient client, RequestChunkPacket rcp)
    {
        if (!ConnectionMap.TryGetValue(client, out var movingPlayer))
        {
            Logger.Warn("Can't find player for chunk send!");
            return;
        }

        if (rcp.X > 15 || rcp.X < 0 || rcp.Z > 15 || rcp.Z < 0)
        {
            Logger.Warn("Player " + client.ClientID + " tried to get invalid chunk! (" + rcp.X + " ," + rcp.Z + ")");
            return;
        }
        ChunkDataPacket cdp = new ChunkDataPacket();
        cdp.X = rcp.X;
        cdp.Z = rcp.Z;
        byte[] cd = new byte[16 * 16 * 128 + 16 * 16 * 64 + 16 * 16];
        int l = 0;
        Chunk c = World._chunks[rcp.X, rcp.Z];

        for (int z = 0; z < 16; ++z)
        {
            for (int x = 0; x < 16; ++x)
            {
                cd[l++] = (byte)0xff;

                for (int y = 0; y < 8; ++y)
                {
                    for (int yB = 0; yB < 16; ++yB)
                    {
                        byte id = c.BlockData[x, z, y * 16 + yB];
                        cd[l++] = id;
                    }

                    for (int yB = 0; yB < 8; ++yB)
                    {
                        byte meta = (byte)(c.BlockMetadata[x, z, y * 16 + yB] + (c.BlockMetadata[x, z, y * 16 + yB + 1] << 4));
                        cd[l++] = meta;
                    }
                }
            }
        }
        cdp.ChunkData = cd;

        client.Send(cdp);

        //  SendAll(new MessagePacket { Username = "Test", Message = "Test1" }) ;
    }
}

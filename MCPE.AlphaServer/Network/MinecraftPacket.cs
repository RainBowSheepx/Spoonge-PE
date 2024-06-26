﻿using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.RakNet;
using SpoongePE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Network;

public class LoginRequestPacket : MinecraftPacket
{
    public string Username;
    public int Protocol1;
    public int Protocol2;
    public uint ClientId;
    public string RealmsData;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Username = reader.String();
        Protocol1 = reader.Int();
        Protocol2 = reader.Int();
        ClientId = reader.UInt();
        RealmsData = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.LoginRequest);
        writer.String(Username);
        writer.Int(Protocol1);
        writer.Int(Protocol2);
        writer.UInt(ClientId);
        writer.String(RealmsData);
    }
}

public class LoginResponsePacket : MinecraftPacket
{
    public LoginStatus Status;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Status = (LoginStatus)reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.LoginResponse);
        writer.Int((int)Status);
    }

    public enum LoginStatus : int
    {
        VersionsMatch,
        ClientOutdated,
        ServerOutdated,
    }

    public static LoginStatus StatusFor(int protocol1, int protocol2, int version)
    {
        if (protocol1 != protocol2 || protocol1 < version)
            return LoginStatus.ClientOutdated;
        if (protocol1 > version)
            return LoginStatus.ServerOutdated;
        return LoginStatus.VersionsMatch;
    }
}

public class ReadyPacket : MinecraftPacket
{
    public byte Status;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Status = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Ready);
        writer.Byte(Status);
    }
}

public class MessagePacket : MinecraftPacket
{
    public string Username;
    public string Message;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Username = reader.String();
        Message = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Message);
        writer.String(Username);
        writer.String(Message);
    }
}

public class SetTimePacket : MinecraftPacket
{
    public int Time;
    public bool started = true; // From PMMP
    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Time = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetTime);
        writer.Int(Time);
        writer.Byte(started == true ? (byte)0x80 : (byte)0x00); // From PMMP
    }
}

public class StartGamePacket : MinecraftPacket
{
    public int Seed;
    public int GeneratorVersion;
    public int Gamemode;
    public int EntityId;
    public Vector3 Pos;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Seed = reader.Int();
        GeneratorVersion = reader.Int();
        Gamemode = reader.Int();
        EntityId = reader.Int();
        Pos = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.StartGame);
        writer.Int(Seed);
        writer.Int(GeneratorVersion);
        writer.Int(Gamemode);
        writer.Int(EntityId);
        writer.Vector3(Pos);
    }
}

public class AddMobPacket : MinecraftPacket
{
    public int EntityId;
    public int MobType;
    public Vector3 Pos;
    public byte Yaw;

    public byte Pitch;
    // public SyncedEntityData Metadata;


    public AddMobPacket() { }

    public AddMobPacket(EntityLiving ent)
    {
        EntityId = ent.EntityID;
        MobType = EntityList.GetEntityID(ent);
        Pos = new Vector3(ent.posX, ent.posY, ent.posZ);
        Yaw = (byte)ent.rotationYaw;
        Pitch = (byte)ent.rotationPitch;
    }

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        MobType = reader.Int();
        Pos = reader.Vector3();
        Yaw = reader.Byte();
        Pitch = reader.Byte();
        // Metadata = default; // TODO
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AddMob);
        writer.Int(EntityId);
        writer.Int(MobType);
        writer.Vector3(Pos);
        writer.Byte(Yaw);
        writer.Byte(Pitch);
        // TODO Metadata
    }
}

public class AddPlayerPacket : MinecraftPacket
{
    public ulong PlayerId;
    public string Username;
    public int EntityId;
    public Vector3 Pos;
    public byte Yaw;
    public byte Pitch;
    public ushort ItemId;
    public ushort ItemAuxValue;

    public byte[] Metadata = {
        0x00, 0x00, 0x21, 0x2c, 0x01, 0x10, 0x00, 0xd1,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x7f
    };

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        PlayerId = reader.ULong();
        Username = reader.String();
        EntityId = reader.Int();
        Pos = reader.Vector3();
        Yaw = reader.Byte();
        Pitch = reader.Byte();
        ItemId = reader.UShort();
        ItemAuxValue = reader.UShort();
        Metadata = reader.Remaining().ToArray();

    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AddPlayer);
        writer.ULong(PlayerId);
        writer.String(Username);
        writer.Int(EntityId);
        writer.Vector3(Pos);
        writer.Byte(Yaw);
        writer.Byte(Pitch);
        writer.UShort(ItemId);
        writer.UShort(ItemAuxValue);
        writer.RawData(Metadata);
    }
}

public class RemovePlayerPacket : MinecraftPacket
{
    public int EntityId;
    public ulong PlayerId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        PlayerId = reader.ULong();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.RemovePlayer);
        writer.Int(EntityId);
        writer.ULong(PlayerId);
    }
}

public class AddEntityPacket : MinecraftPacket
{
    public int EntityId;
    public byte EntityType;
    public Vector3 Pos;
    public int Moved;
    public Vector3 Velocity;

    public AddEntityPacket() { }


    public AddEntityPacket(Entity ent)
    {
        EntityId = ent.EntityID;
        EntityType = (byte)EntityList.GetEntityID(ent);
        Pos = new Vector3(ent.posX, ent.posY, ent.posZ);
        Moved = 0;
        if (ent.motionX + ent.motionY + ent.motionZ > 0) // check
        {
            Moved = 1;
            Velocity = new Vector3(ent.motionX, ent.motionY, ent.motionZ);
        }

    }


    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        EntityType = reader.Byte();
        Pos = reader.Vector3();
        Moved = reader.Int();
        if (Moved > 0)
            Velocity = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AddEntity);
        writer.Int(EntityId);
        writer.Byte(EntityType);
        writer.Vector3(Pos);
        writer.Int(Moved);
        if (Moved > 0)
            writer.Vector3(Velocity);
    }
}

public class RemoveEntityPacket : MinecraftPacket
{
    public int EntityId;

    public RemoveEntityPacket() { }

    public RemoveEntityPacket(int entityID)
    {
        EntityId = entityID;
    }

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.RemoveEntity);
        writer.Int(EntityId);
    }
}

public class AddItemEntityPacket : MinecraftPacket
{
    public int EntityId;

    public ItemStack Item;
    public Vector3 Pos;
    public byte Yaw;
    public byte Pitch;
    public byte Roll;

    public AddItemEntityPacket() { }

    public AddItemEntityPacket(EntityItem var1)
    {
        this.EntityId = var1.EntityID;
        this.Item = var1.item;
        this.Pos.X = MathHelper.floor_double(var1.posX * 32.0D);
        this.Pos.Y = MathHelper.floor_double(var1.posY * 32.0D);
        this.Pos.Z = MathHelper.floor_double(var1.posZ * 32.0D);
        this.Yaw = (byte)((int)(var1.motionX * 128.0D));
        this.Pitch = (byte)((int)(var1.motionY * 128.0D));
        this.Roll = (byte)((int)(var1.motionZ * 128.0D));
    }

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Item = reader.Slot(); // TODO
        Pos = reader.Vector3();
        Yaw = reader.Byte();
        Pitch = reader.Byte();
        Roll = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AddItemEntity);
        writer.Int(EntityId);
        writer.Slot(Item);
        writer.Vector3(Pos);
        writer.Byte(Yaw);
        writer.Byte(Pitch);
        writer.Byte(Roll);
    }
}

public class TakeItemEntityPacket : MinecraftPacket
{
    public int Target;
    public int EntityId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Target = reader.Int();
        EntityId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.TakeItemEntity);
        writer.Int(Target);
        writer.Int(EntityId);
    }
}

public class MoveEntityPacket : MinecraftPacket
{
    public int EntityId;
    public Vector3 Pos;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Pos = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.MoveEntity);
        writer.Int(EntityId);
        writer.Vector3(Pos);
    }
}

public class MoveEntityPosRotPacket : MinecraftPacket
{
    public int EntityId;
    public Vector3 Pos;
    public byte Yaw;
    public byte Pitch;

    public MoveEntityPosRotPacket() { }

    public MoveEntityPosRotPacket(Entity ent, byte var5, byte var6) // alias Packet32EntityLook
    {
        EntityId = ent.EntityID;
        Pos = new Vector3(ent.posX, ent.posY, ent.posZ);
        Yaw = var5;
        Pitch = var6;
    }

    public MoveEntityPosRotPacket(Entity ent, byte var5, byte var6, byte var9) // alias Packet31RelEntityMove
    {
        EntityId = ent.EntityID;
        Pos = new Vector3(var5, var6, var9);
        Yaw = (byte)ent.rotationYaw;
        Pitch = (byte)ent.rotationPitch;
    }
    public MoveEntityPosRotPacket(Entity ent, byte var5, byte var6, byte var9, byte var51, byte var61) // alias Packet33RelEntityMoveLook
    {
        EntityId = ent.EntityID;
        Pos = new Vector3(var5, var6, var9);
        Yaw = var51;
        Pitch = var61;
    }
    public MoveEntityPosRotPacket(int entityID, int var2, int var3, int var4, byte var5, byte var6) // alias Packet34EntityTeleport
    {
        EntityId = entityID;
        Pos = new Vector3(var2, var3, var4);
        Yaw = var5;
        Pitch = var6;
    }



    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Pos = reader.Vector3();
        Yaw = reader.Byte();
        Pitch = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.MoveEntityPosRot);
        writer.Int(EntityId);
        writer.Vector3(Pos);
        writer.Byte(Yaw);
        writer.Byte(Pitch);
    }
}

public class RotateHeadPacket : MinecraftPacket
{
    public int EntityId;
    public byte Yaw;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Yaw = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.RotateHead);
        writer.Int(EntityId);
        writer.Byte(Yaw);
    }
}

public class MovePlayerPacket : MinecraftPacket
{
    public int EntityId;
    public Vector3 Pos;
    public Vector3 Rot;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Pos = reader.Vector3();
        Rot = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.MovePlayer);
        writer.Int(EntityId);
        writer.Vector3(Pos);
        writer.Vector3(Rot);
    }
}

public class PlaceBlockPacket : MinecraftPacket
{
    public int EntityId;
    public int X;
    public int Z;
    public byte Y;
    public byte Block;
    public byte Meta;
    public byte Face;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        X = reader.Int();
        Z = reader.Int();
        Y = reader.Byte();
        Block = reader.Byte();
        Meta = reader.Byte();
        Face = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.PlaceBlock);
        writer.Int(EntityId);
        writer.Int(X);
        writer.Int(Z);
        writer.Byte(Y);
        writer.Byte(Block);
        writer.Byte(Meta);
        writer.Byte(Face);
    }
}

public class RemoveBlockPacket : MinecraftPacket
{
    public int EntityId;
    public int X;
    public int Z;
    public byte Y;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        X = reader.Int();
        Z = reader.Int();
        Y = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.RemoveBlock);
        writer.Int(EntityId);
        writer.Int(X);
        writer.Int(Z);
        writer.Byte(Y);
    }
}

public class UpdateBlockPacket : MinecraftPacket
{
    public int EntityId;
    public int X;
    public int Z;
    public byte Y;
    public byte Block;
    public byte Meta;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        X = reader.Int();
        Z = reader.Int();
        Y = reader.Byte();
        Block = reader.Byte();
        Meta = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.UpdateBlock);
        writer.Int(EntityId);
        writer.Int(X);
        writer.Int(Z);
        writer.Byte(Y);
        writer.Byte(Block);
        writer.Byte(Meta);
    }
}

public class AddPaintingPacket : MinecraftPacket
{
    public int EntityId;
    public int X;
    public int Y;
    public int Direction;
    public string Title;

    public AddPaintingPacket() { }

    public AddPaintingPacket(EntityPainting painting)
    {

    }

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        X = reader.Int();
        Y = reader.Int();
        Direction = reader.Int();
        Title = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AddPainting);
        writer.Int(EntityId);
        writer.Int(X);
        writer.Int(Y);
        writer.Int(Direction);
        writer.String(Title);
    }
}

public class ExplodePacket : MinecraftPacket
{
    public Vector3 Pos;
    public float Radius;
    public int CountIncompleteSetToZero;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Pos = reader.Vector3();
        Radius = reader.Float();
        CountIncompleteSetToZero = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Explode);
        writer.Vector3(Pos);
        writer.Float(Radius);
        writer.Int(CountIncompleteSetToZero);
    }
}

public class LevelEventPacket : MinecraftPacket
{
    public ushort EventId;
    public ushort X;
    public ushort Y;
    public ushort Z;
    public int EventData;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EventId = reader.UShort();
        X = reader.UShort();
        Y = reader.UShort();
        Z = reader.UShort();
        EventData = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.LevelEvent);
        writer.UShort(EventId);
        writer.UShort(X);
        writer.UShort(Y);
        writer.UShort(Z);
        writer.Int(EventData);
    }
}

public class TileEventPacket : MinecraftPacket
{
    public int X;
    public int Y;
    public int Case1;
    public int Case2;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.Int();
        Y = reader.Int();
        Case1 = reader.Int();
        Case2 = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.TileEvent);
        writer.Int(X);
        writer.Int(Y);
        writer.Int(Case1);
        writer.Int(Case2);
    }
}

public class EntityEventPacket : MinecraftPacket
{
    public int EntityId;
    public byte EventId;

    public const byte ENTITY_DAMAGE = 2;
    public const byte ENTITY_DEAD = 3;
    public const byte ENTITY_ANIM_10 = 10;
    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        EventId = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.EntityEvent);
        writer.Int(EntityId);
        writer.Byte(EventId);
    }
}

public class RequestChunkPacket : MinecraftPacket
{
    public int X;
    public int Z;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.Int();
        Z = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.RequestChunk);
        writer.Int(X);
        writer.Int(Z);
    }
}

public class ChunkDataPacket : MinecraftPacket
{
    public int X;
    public int Z;
    public byte[] ChunkData;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.Int();
        Z = reader.Int();
        Data = default; // TODO
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ChunkData);
        writer.Int(X);
        writer.Int(Z);
        writer.RawData(ChunkData);
    }
}

public class PlayerEquipmentPacket : MinecraftPacket
{
    public int EntityId;
    public ushort Item;
    public ushort Meta;
    public byte Slot;

    public PlayerEquipmentPacket() { }

    public PlayerEquipmentPacket(int entityID, int var7, ItemStack itemStack)
    {
        EntityId = entityID;
        Slot = (byte)var7;
        Item = (ushort)itemStack.itemID;
        Meta = (ushort)itemStack.itemDamage;
    }

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Item = reader.UShort();
        Meta = reader.UShort();
        Slot = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.PlayerEquipment);
        writer.Int(EntityId);
        writer.UShort(Item);
        writer.UShort(Meta);
        writer.Byte(Slot);
    }
}

public class PlayerArmorEquipmentPacket : MinecraftPacket
{
    public int EntityId;
    public ushort Slot1;
    public ushort Slot2;
    public ushort Slot3;
    public ushort Slot4;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Slot1 = reader.UShort();
        Slot2 = reader.UShort();
        Slot3 = reader.UShort();
        Slot4 = reader.UShort();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.PlayerArmorEquipment);
        writer.Int(EntityId);
        writer.UShort(Slot1);
        writer.UShort(Slot2);
        writer.UShort(Slot3);
        writer.UShort(Slot4);
    }
}

public class InteractPacket : MinecraftPacket
{
    public byte Action;
    public int EntityId;
    public int TargetId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Action = reader.Byte();
        EntityId = reader.Int();
        TargetId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Interact);
        writer.Byte(Action);
        writer.Int(EntityId);
        writer.Int(TargetId);
    }
}

public class UseItemPacket : MinecraftPacket
{
    public int X;
    public int Y;
    public int Face;
    public ushort Block;
    public byte Meta;
    public int Id;
    public Vector3 FPos;
    public Vector3 Pos;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.Int();
        Y = reader.Int();
        Face = reader.Int();
        Block = reader.UShort();
        Meta = reader.Byte();
        Id = reader.Int();
        FPos = reader.Vector3();
        Pos = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.UseItem);
        writer.Int(X);
        writer.Int(Y);
        writer.Int(Face);
        writer.UShort(Block);
        writer.Byte(Meta);
        writer.Int(Id);
        // TODO FPos // OK
        writer.Vector3(FPos);
        writer.Vector3(Pos);
    }
}

public class PlayerActionPacket : MinecraftPacket
{
    public int Action;
    public int X;
    public int Y;
    public int Face;
    public int EntityId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Action = reader.Int();
        X = reader.Int();
        Y = reader.Int();
        Face = reader.Int();
        EntityId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.PlayerAction);
        writer.Int(Action);
        writer.Int(X);
        writer.Int(Y);
        writer.Int(Face);
        writer.Int(EntityId);
    }
}

public class HurtArmorPacket : MinecraftPacket
{
    public byte Armor;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Armor = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.HurtArmor);
        writer.Byte(Armor);
    }
}

public class SetEntityDataPacket : MinecraftPacket
{
    public int EntityId;
    // public SyncedEntityData Metadata;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        // Metadata = default; // TODO
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetEntityData);
        writer.Int(EntityId);
        // TODO Metadata
    }
}

public class SetEntityMotionPacket : MinecraftPacket
{
    // public byte Unk0; // where?
    public int EntityId;
    public ushort X;
    public ushort Y;
    public ushort Z;

    public SetEntityMotionPacket() { }

    public SetEntityMotionPacket(Entity trackedEntity) : this(trackedEntity.EntityID, trackedEntity.motionX, trackedEntity.motionY, trackedEntity.motionZ)
    {
    }

    public SetEntityMotionPacket(int entityId, float motionX, float motionY, float motionZ)
    {
        // Unk0 = 0;
        EntityId = entityId;
        X = (ushort)motionX;
        Y = (ushort)motionY;
        Z = (ushort)motionZ;
    }
    public SetEntityMotionPacket(int entityId, double motionX, double motionY, double motionZ)
    {
        // Unk0 = 0;
        EntityId = entityId;
        X = (ushort)motionX;
        Y = (ushort)motionY;
        Z = (ushort)motionZ;
    }
    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
                       //  Unk0 = reader.Byte();
        EntityId = reader.Int();
        X = reader.UShort();
        Y = reader.UShort();
        Z = reader.UShort();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetEntityMotion);
        //  writer.Byte(Unk0);
        writer.Int(EntityId);
        writer.UShort((ushort)(X * 8000));
        writer.UShort((ushort)(Y * 8000));
        writer.UShort((ushort)(Z * 8000));
    }
}

public class SetRidingPacket : MinecraftPacket
{
    public int EntityId;
    public int TargetId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        TargetId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetRiding);
        writer.Int(EntityId);
        writer.Int(TargetId);
    }
}

public class SetHealthPacket : MinecraftPacket
{
    public sbyte Health;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Health = (sbyte)reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetHealth);
        writer.Byte((byte)Health);
    }
}

public class SetSpawnPositionPacket : MinecraftPacket
{
    public int X;
    public int Z;
    public byte Y;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.Int();
        Z = reader.Int();
        Y = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetSpawnPosition);
        writer.Int(X);
        writer.Int(Z);
        writer.Byte(Y);
    }
}

public class AnimatePacket : MinecraftPacket
{
    public byte Action;
    public int EntityId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Action = reader.Byte();
        EntityId = reader.Int();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Animate);
        writer.Byte(Action);
        writer.Int(EntityId);
    }
}

public class RespawnPacket : MinecraftPacket
{
    public int EntityId;
    public Vector3 Pos;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Pos = reader.Vector3();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Respawn);
        writer.Int(EntityId);
        writer.Vector3(Pos);
    }
}

public class SendInventoryPacket : MinecraftPacket
{
    public int EntityId;
    public byte WindowId;
    public List<ItemStack> Items;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        WindowId = reader.Byte();
        Items = reader.Items();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SendInventory);
        writer.Int(EntityId);
        writer.Byte(WindowId);
        writer.Items(Items);
    }
}

public class DropItemPacket : MinecraftPacket
{
    public int EntityId;

    public byte Unk0;
    public ItemStack Item;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        EntityId = reader.Int();
        Unk0 = reader.Byte();
        Item = reader.Slot(); // TODO
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.DropItem);
        writer.Int(EntityId);
        writer.Byte(Unk0);
        writer.Slot(Item);
    }
}

public class ContainerOpenPacket : MinecraftPacket
{
    public byte WindowId;
    public byte ContainerType;
    public byte Slot;
    public string Title;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
        ContainerType = reader.Byte();
        Slot = reader.Byte();
        Title = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerOpen);
        writer.Byte(WindowId);
        writer.Byte(ContainerType);
        writer.Byte(Slot);
        writer.String(Title);
    }
}

public class ContainerClosePacket : MinecraftPacket
{
    public byte WindowId;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerClose);
        writer.Byte(WindowId);
    }
}

public class ContainerSetSlotPacket : MinecraftPacket
{
    public byte WindowId;

    public ushort Slot;
    public ItemStack Item;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
        Slot = reader.UShort();
        Item = reader.Slot(); // TODO
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerSetSlot);
        writer.Byte(WindowId);
        writer.UShort(Slot);
        writer.Slot(Item);
    }
}

public class ContainerSetDataPacket : MinecraftPacket
{
    public byte WindowId;
    public ushort Property;
    public ushort Value;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
        Property = reader.UShort();
        Value = reader.UShort();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerSetData);
        writer.Byte(WindowId);
        writer.UShort(Property);
        writer.UShort(Value);
    }
}

public class ContainerSetContentPacket : MinecraftPacket
{
    public byte WindowId;
    public List<ItemStack> Items;
    public List<int> Hotbar; // ???

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
        Items = reader.Items();

        if (WindowId != 0)
            return;

        Hotbar = reader.Hotbar();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerSetContent);
        writer.Byte(WindowId);
        writer.Items(Items);

        if (WindowId != 0)
            return;

        writer.Hotbar(Hotbar);
    }
}

public class ContainerAckPacket : MinecraftPacket
{
    public byte WindowId;
    public ushort Unk0;
    public byte Unk1;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        WindowId = reader.Byte();
        Unk0 = reader.UShort();
        Unk1 = reader.Byte();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.ContainerAck);
        writer.Byte(WindowId);
        writer.UShort(Unk0);
        writer.Byte(Unk1);
    }
}

public class ChatPacket : MinecraftPacket
{
    public string Message;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Message = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.Chat);
        writer.String(Message);
    }
}

public class AdventureSettingsPacket : MinecraftPacket
{ // need to check
    public ushort X;
    public byte Y;
    public ushort Z;
    public string Lines;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        X = reader.UShort();
        Y = reader.Byte();
        Z = reader.UShort();
        Lines = reader.String();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.AdventureSettings);
        writer.UShort(X);
        writer.Byte(Y);
        writer.UShort(Z);
        writer.String(Lines);
    }
}

public class SetTileEntityDataPacket : MinecraftPacket
{ // need to check 
    public byte Unk0;
    public uint Unk1;

    public override void Decode(ref DataReader reader)
    {
        reader.Byte(); // Packet type.
        Unk0 = reader.Byte();
        Unk1 = reader.UInt();
    }

    public override void Encode(ref DataWriter writer)
    {
        writer.Byte((byte)MinecraftPacketType.SetTileEntityData);
        writer.Byte(Unk0);
        writer.UInt(Unk1);
    }
}
public class UnknowPacket : MinecraftPacket
{
    public int packetID = -1;
    public byte[] buffer;

    public override void Decode(ref DataReader reader)
    {

    }

    public override void Encode(ref DataWriter writer)
    {
        writer.RawData(buffer); // Emulate raw buffer from PMMP

    }
}
public abstract class MinecraftPacket : UserPacket
{
    public static MinecraftPacket Parse(ReadOnlyMemory<byte> data)
    {
        MinecraftPacket packet = data.Span[0] switch
        {
            (byte)MinecraftPacketType.LoginRequest => new LoginRequestPacket(),
            (byte)MinecraftPacketType.LoginResponse => new LoginResponsePacket(),
            (byte)MinecraftPacketType.Ready => new ReadyPacket(),
            (byte)MinecraftPacketType.Message => new MessagePacket(),
            (byte)MinecraftPacketType.SetTime => new SetTimePacket(),
            (byte)MinecraftPacketType.StartGame => new StartGamePacket(),
            (byte)MinecraftPacketType.AddMob => new AddMobPacket(),
            (byte)MinecraftPacketType.AddPlayer => new AddPlayerPacket(),
            (byte)MinecraftPacketType.RemovePlayer => new RemovePlayerPacket(),
            (byte)MinecraftPacketType.AddEntity => new AddEntityPacket(),
            (byte)MinecraftPacketType.RemoveEntity => new RemoveEntityPacket(),
            (byte)MinecraftPacketType.AddItemEntity => new AddItemEntityPacket(),
            (byte)MinecraftPacketType.TakeItemEntity => new TakeItemEntityPacket(),
            (byte)MinecraftPacketType.MoveEntity => new MoveEntityPacket(),
            (byte)MinecraftPacketType.MoveEntityPosRot => new MoveEntityPosRotPacket(),
            (byte)MinecraftPacketType.RotateHead => new RotateHeadPacket(),
            (byte)MinecraftPacketType.MovePlayer => new MovePlayerPacket(),
            (byte)MinecraftPacketType.PlaceBlock => new PlaceBlockPacket(),
            (byte)MinecraftPacketType.RemoveBlock => new RemoveBlockPacket(),
            (byte)MinecraftPacketType.UpdateBlock => new UpdateBlockPacket(),
            (byte)MinecraftPacketType.AddPainting => new AddPaintingPacket(),
            (byte)MinecraftPacketType.Explode => new ExplodePacket(),
            (byte)MinecraftPacketType.LevelEvent => new LevelEventPacket(),
            (byte)MinecraftPacketType.TileEvent => new TileEventPacket(),
            (byte)MinecraftPacketType.EntityEvent => new EntityEventPacket(),
            (byte)MinecraftPacketType.RequestChunk => new RequestChunkPacket(),
            (byte)MinecraftPacketType.ChunkData => new ChunkDataPacket(),
            (byte)MinecraftPacketType.PlayerEquipment => new PlayerEquipmentPacket(),
            (byte)MinecraftPacketType.PlayerArmorEquipment => new PlayerArmorEquipmentPacket(),
            (byte)MinecraftPacketType.Interact => new InteractPacket(),
            (byte)MinecraftPacketType.UseItem => new UseItemPacket(),
            (byte)MinecraftPacketType.PlayerAction => new PlayerActionPacket(),
            (byte)MinecraftPacketType.HurtArmor => new HurtArmorPacket(),
            (byte)MinecraftPacketType.SetEntityData => new SetEntityDataPacket(),
            (byte)MinecraftPacketType.SetEntityMotion => new SetEntityMotionPacket(),
            (byte)MinecraftPacketType.SetRiding => new SetRidingPacket(),
            (byte)MinecraftPacketType.SetHealth => new SetHealthPacket(),
            (byte)MinecraftPacketType.SetSpawnPosition => new SetSpawnPositionPacket(),
            (byte)MinecraftPacketType.Animate => new AnimatePacket(),
            (byte)MinecraftPacketType.Respawn => new RespawnPacket(),
            (byte)MinecraftPacketType.SendInventory => new SendInventoryPacket(),
            (byte)MinecraftPacketType.DropItem => new DropItemPacket(),
            (byte)MinecraftPacketType.ContainerOpen => new ContainerOpenPacket(),
            (byte)MinecraftPacketType.ContainerClose => new ContainerClosePacket(),
            (byte)MinecraftPacketType.ContainerSetSlot => new ContainerSetSlotPacket(),
            (byte)MinecraftPacketType.ContainerSetData => new ContainerSetDataPacket(),
            (byte)MinecraftPacketType.ContainerSetContent => new ContainerSetContentPacket(),
            (byte)MinecraftPacketType.ContainerAck => new ContainerAckPacket(),
            (byte)MinecraftPacketType.Chat => new ChatPacket(),
            (byte)MinecraftPacketType.AdventureSettings => new AdventureSettingsPacket(),
            (byte)MinecraftPacketType.SetTileEntityData => new SetTileEntityDataPacket(),
            (byte)MinecraftPacketType.Unknown => new UnknowPacket(),
            _ => null,
        };

        if (packet is null) return null;

        var reader = new DataReader(data);
        packet.Decode(ref reader);

        return packet;
    }
}

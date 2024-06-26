using SpoongePE.Core.Game.BlockBase.impl;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.Game.utils.random;
using SpoongePE.Core.Network;
using SpoongePE.Core.Utils;
using System;
using System.Collections.Generic;

namespace SpoongePE.Core.Game.BlockBase;

public abstract class Block
{

    public static Block[] blocks = new Block[256];
    public static bool[] shouldTick = new bool[256];
    public static int[] lightOpacity = new int[256];

    public static SolidBlock stone = new SolidBlock(1, Material.stone).setBlockName("Stone"); //love old mcje code btw <<<
    public static SolidBlock grass = new SolidBlock(2, Material.dirt).setBlockName("Grass");
    public static SolidBlock dirt = new SolidBlock(3, Material.dirt).setBlockName("Dirt");
    public static SolidBlock cobblestone = new SolidBlock(4, Material.stone).setBlockName("Cobblestone");
    public static SolidBlock planks = new SolidBlock(5, Material.wood).setBlockName("Planks");
    public static PlantBlock sapling = new PlantBlock(6, Material.plant);
/*    public static PlantBlock spruceSapling = new PlantBlock(6, Material.plant, 1);
    public static PlantBlock birchSapling = new PlantBlock(6, Material.plant, 2);*/
    public static SolidBlock bedrock = new SolidBlock(7, Material.stone).setBlockName("Bedrock");
    public static LiquidStaticBlock waterFlowing = new LiquidStaticBlock(8, Material.water);
    public static LiquidStaticBlock waterStill = new LiquidStaticBlock(9, Material.water);
    public static LiquidStaticBlock lavaFlowing = new LiquidStaticBlock(10, Material.lava);
    public static LiquidStaticBlock lavaStill = new LiquidStaticBlock(11, Material.lava);
    public static SolidBlock sand = new SolidBlock(12, Material.sand);
    public static SolidBlock gravel = new SolidBlock(13, Material.sand);
    public static SolidBlock oreGold = new SolidBlock(14, Material.stone);
    public static SolidBlock oreIron = new SolidBlock(15, Material.stone);
    public static SolidBlock oreCoal = new SolidBlock(16, Material.stone);
    public static SolidBlock log = new SolidBlock(17, Material.wood);
    public static SolidBlock leaves = new SolidBlock(18, Material.leaves);
    public static SpongeBlock sponge = new SpongeBlock(19, Material.sponge);
    public static SolidBlock glass = new SolidBlock(20, Material.glass);
    public static SolidBlock lapisOre = new SolidBlock(21, Material.stone);
    public static SolidBlock lapisBlock = new SolidBlock(22, Material.metal);

    public static SolidBlock sandStone = new SolidBlock(24, Material.stone);

    public static BedBlock bedBlock = new BedBlock(26, Material.cloth);
    public static RailBlock poweredRail = new RailBlock(27, Material.metal);

    public static DecorationBlock cobweb = new DecorationBlock(30, Material.cloth);
/*    public static PlantBlock tallGrass = new PlantBlock(31, Material.plant, 1);
    public static PlantBlock fern = new PlantBlock(31, Material.plant, 2);*/
    public static PlantBlock deadBush = new PlantBlock(32, Material.plant);
    public static PlantBlock yellowFlowerBlock = new PlantBlock(37, Material.plant);
    public static PlantBlock rose = new PlantBlock(38, Material.plant);
    public static PlantBlock brownMushroom = new PlantBlock(39, Material.plant);
    public static PlantBlock redMushroom = new PlantBlock(40, Material.plant);
    public static SolidBlock goldBlock = new SolidBlock(41, Material.metal);
    public static SolidBlock ironBlock = new SolidBlock(42, Material.metal);
    public static SolidBlock fullStoneSlab = new SolidBlock(43, Material.stone);
    public static SolidBlock stoneSlab = new SolidBlock(44, Material.stone);
/*    public static SolidBlock cobbleSlab = new SolidBlock(44, Material.stone, 3);
    public static SolidBlock brickSlab = new SolidBlock(44, Material.stone, 4);
    public static SolidBlock sandstoneSlab = new SolidBlock(44, Material.stone, 1);
    public static SolidBlock stonebrickSlab = new SolidBlock(44, Material.stone, 5);
    public static SolidBlock quartzSlab = new SolidBlock(44, Material.stone, 6);*/
    public static SolidBlock brick = new SolidBlock(45, Material.stone);
    public static SolidBlock tnt = new SolidBlock(46, Material.stone);
    public static SolidBlock bookshelf = new SolidBlock(47, Material.wood);
    public static SolidBlock mossyStone = new SolidBlock(48, Material.stone);
    public static SolidBlock obsidian = new SolidBlock(49, Material.stone);
    public static DecorationBlock torch = new DecorationBlock(50, Material.decoration);
    public static DecorationBlock fire = new DecorationBlock(51, Material.fire);

    public static SolidBlock woodStairs = new SolidBlock(53, Material.wood);
    public static ChestBlock chest = new ChestBlock(54, Material.wood);

    public static SolidBlock oreDiamond = new SolidBlock(56, Material.stone);
    public static SolidBlock diamondBlock = new SolidBlock(57, Material.metal);
    public static SolidBlock workdbench = new SolidBlock(58, Material.wood);
    public static PlantBlock wheatBlock = new PlantBlock(59, Material.plant); //
    public static SolidBlock farmland = new SolidBlock(60, Material.dirt);
    public static SolidBlock furnace = new SolidBlock(61, Material.stone);
    public static SolidBlock burningFurnace = new SolidBlock(62, Material.stone);
    public static DecorationBlock signPost = new DecorationBlock(63, Material.wood);
    public static DoorBlock woodenDoor = new DoorBlock(64, Material.wood);//
    public static DecorationBlock ladder = new DecorationBlock(65, Material.wood); //
    public static RailBlock rail = new RailBlock(66, Material.metal);
    public static StairsBlock cobbleStairs = new StairsBlock(67, Material.stone);
    public static DecorationBlock wallSign = new DecorationBlock(68, Material.wood);
    public static DoorBlock ironDoor = new DoorBlock(71, Material.metal);
    public static SolidBlock redstoneOre = new SolidBlock(73, Material.stone);
    public static SolidBlock glowingRedstoneOre = new SolidBlock(74, Material.stone);
    public static DecorationBlock snowLayer = new DecorationBlock(78, Material.decoration);
    public static SolidBlock ice = new SolidBlock(79, Material.ice);
    public static SolidBlock snow = new SolidBlock(80, Material.snow);
    public static SolidBlock cactus = new SolidBlock(81, Material.cactus);
    public static SolidBlock clay = new SolidBlock(82, Material.clay);
    public static PlantBlock reeds = new PlantBlock(83, Material.plant);
    public static SolidBlock fence = new SolidBlock(85, Material.wood);
    public static SolidBlock pumpkin = new SolidBlock(86, Material.vegetable);
    public static SolidBlock netherrack = new SolidBlock(87, Material.stone);
   // public static SolidBlock soulSand = new SolidBlock(88, Material.sand); NostalgiaCore moment
    public static SolidBlock glowstone = new SolidBlock(89, Material.stone);

    public static SolidBlock jack_o_lantern = new SolidBlock(91, Material.vegetable);
    public static SolidBlock cake = new SolidBlock(92, Material.vegetable); // yez
    public static SolidBlock invisibleBedrock = new SolidBlock(95, Material.stone); //TODO destructible/indestructible
    public static SolidBlock trapdoor = new SolidBlock(96, Material.stone);
    public static SolidBlock stoneBrick = new SolidBlock(98, Material.stone);
    //public static SolidBlock mossyStoneBrick = new SolidBlock(98, Material.stone, 1);

    public static SolidBlock ironBar = new SolidBlock(101, Material.metal);
    public static SolidBlock glassPane = new SolidBlock(102, Material.glass);
    public static SolidBlock melon = new SolidBlock(103, Material.vegetable);
    public static PlantBlock pumpkinStem = new PlantBlock(104, Material.plant);
    public static PlantBlock melonStem = new PlantBlock(105, Material.plant);

    public static SolidBlock fenceGate = new SolidBlock(107, Material.wood);
    public static SolidBlock brickStairs = new SolidBlock(108, Material.stone);
    public static SolidBlock stoneBrickStairs = new SolidBlock(109, Material.stone);

    public static SolidBlock netherBrick = new SolidBlock(112, Material.stone);
    public static SolidBlock netherBrickStairs = new SolidBlock(114, Material.stone);
    public static SolidBlock sandstoneStairs = new SolidBlock(128, Material.stone);
    public static SolidBlock spruceStairs = new SolidBlock(134, Material.wood);
    public static SolidBlock birchStairs = new SolidBlock(135, Material.wood);
    public static SolidBlock jungleStairs = new SolidBlock(136, Material.wood);

    public static SolidBlock cobbleWall = new SolidBlock(139, Material.stone);
    public static PlantBlock carrot = new PlantBlock(141, Material.plant);
    public static PlantBlock potato = new PlantBlock(142, Material.plant);

    public static SolidBlock quartz = new SolidBlock(155, Material.stone);
    public static SolidBlock quartzStairs = new SolidBlock(156, Material.stone);
    public static SolidBlock doubleWoodSlab = new SolidBlock(157, Material.wood);
    public static SolidBlock woodSlab = new SolidBlock(158, Material.wood);

    public static SolidBlock hayBale = new SolidBlock(170, Material.wood);
    public static SolidBlock coalBlock = new SolidBlock(173, Material.stone);
    public static PlantBlock beetroot = new PlantBlock(244, Material.plant);
    public static SolidBlock stoneCutter = new SolidBlock(245, Material.stone);
    public static SolidBlock glowingObsidian = new SolidBlock(246, Material.stone); // wtf???
    public static SolidBlock netherReactor = new SolidBlock(247, Material.stone);
    public static SolidBlock updateGame = new SolidBlock(248, Material.stone);
    public static SolidBlock updateGame2 = new SolidBlock(249, Material.stone);
    public static SolidBlock reserved6 = new SolidBlock(255, Material.stone); // wtf x2 


    public static WoolBlock wool = new WoolBlock(35, 0); 


    public static CarpetBlock carpet = new CarpetBlock(171, 0);

    //          Монумент моей тупости
    //
    /*    public static WoolBlock carpet_lightgray = new WoolBlock(171, 8);
        public static WoolBlock carpet_gray = new WoolBlock(171, 7);
        public static WoolBlock carpet_black = new WoolBlock(171, 15);
        public static WoolBlock carpet_brown = new WoolBlock(171, 12);
        public static WoolBlock carpet_red = new WoolBlock(171, 14);
        public static WoolBlock carpet_orange = new WoolBlock(171, 1);
        public static WoolBlock carpet_yellow = new WoolBlock(171, 4);
        public static WoolBlock carpet_lime = new WoolBlock(171, 5);
        public static WoolBlock carpet_green = new WoolBlock(171, 13);
        public static WoolBlock carpet_cyan = new WoolBlock(171, 9);
        public static WoolBlock carpet_lightblue = new WoolBlock(171, 3);
        public static WoolBlock carpet_blue = new WoolBlock(171, 11);
        public static WoolBlock carpet_purple = new WoolBlock(171, 10);
        public static WoolBlock carpet_magenta = new WoolBlock(171, 2);
        public static WoolBlock carpet_pink = new WoolBlock(171, 6);*/

    /*    public static WoolBlock wool_lightgray = new WoolBlock(35, 8); //using ids instead of meta =/ // Кто здесь?
        public static WoolBlock wool_gray = new WoolBlock(35, 7);
        public static WoolBlock wool_black = new WoolBlock(35, 15);
        public static WoolBlock wool_brown = new WoolBlock(35, 12);
        public static WoolBlock wool_red = new WoolBlock(35, 14);
        public static WoolBlock wool_orange = new WoolBlock(35, 1);
        public static WoolBlock wool_yellow = new WoolBlock(35, 4);
        public static WoolBlock wool_lime = new WoolBlock(35, 5);
        public static WoolBlock wool_green = new WoolBlock(35, 13);
        public static WoolBlock wool_cyan = new WoolBlock(35, 9);
        public static WoolBlock wool_lightblue = new WoolBlock(35, 3);
        public static WoolBlock wool_blue = new WoolBlock(35, 11);
        public static WoolBlock wool_purple = new WoolBlock(35, 10);
        public static WoolBlock wool_magenta = new WoolBlock(35, 2);
        public static WoolBlock wool_pink = new WoolBlock(35, 6);
        */
    public virtual void onNeighborBlockChanged(World world, int x, int y, int z, int meta) { }
    public virtual void onBlockRemoved(World world, int x, int y, int z)
    {
        world.removeBlock(x, y, z);
        UpdateBlockPacket pk = new UpdateBlockPacket();
        pk.X = x;
        pk.Y = (byte)y;
        pk.Z = z;
        pk.Block = 0;
        pk.Meta = 0;
        world.broadcastPacket(pk);
    }
    public virtual void onBlockRemovedByPlayer(World world, int x, int y, int z, Player player)
    {
        world.removeBlock(x, y, z);
    }

    public virtual void tick(World world, int x, int y, int z, BedrockRandom random)
    {

    }

    public virtual void onBlockAdded(World world, int x, int y, int z)
    {
        
    }

    public void onRemove(World world, int x, int y, int z)
    {
        /*
		 * TODO ice, leaf, stairs, trunk
			IceTile::onRemove(Level *,int,int,int)
			LeafTile::onRemove(Level *,int,int,int)
			StairTile::onRemove(Level *,int,int,int)
			TreeTile::onRemove(Level *,int,int,int)
		 */
    }
    public Block setBlockName(string name)
    {
        this.name = name;
        return this;
    }
    public virtual bool canSurvive(World world, int x, int y, int z)
    {
        return true;
    }

    public virtual void onBlockPlacedByPlayer(World world, int x, int y, int z, int face, Player player)
    {
        world.placeBlockAndNotifyNearby(x, y, z, (byte)blockID);
    }

    public static void Init()
    {
        for (int i = 0; i < 256; ++i)
        {
            if (blocks[i] != null)
            {
                if (Item.itemsList[i] == null)
                {
                    Item.itemsList[i] = new BlockItem(i - 256);
                }
            }
        }

    }

    public bool mayPlace(World world, int x, int y, int z)
    {
        int blockID = world.getBlockIDAt(x, y, z);

        if (blockID == 0) return true;

        return blocks[blockID].material.isLiquid;
    }

    public void setPlacedBy(World world, int x, int y, int z, Player player)
    {

    }

    public void setPlacedOnFace(World world, int x, int y, int z, int side)
    {

    }

    internal void getCollidingBoundingBoxes(World var1, int var2, int var3, int var4, AxisAlignedBB var5, List<AxisAlignedBB> var6)
    {
        AxisAlignedBB var7 = this.getCollisionBoundingBoxFromPool(var1, var2, var3, var4);
        if (var7 != null && var5.intersectsWith(var7))
        {
            var6.Add(var7);
        }
    }

    public AxisAlignedBB getCollisionBoundingBoxFromPool(World var1, int var2, int var3, int var4)
    => AxisAlignedBB.getBoundingBoxFromPool((double)var2 + this.minX, (double)var3 + this.minY, (double)var4 + this.minZ, (double)var2 + this.maxX, (double)var3 + this.maxY, (double)var4 + this.maxZ);
    public void setBlockBounds(float var1, float var2, float var3, float var4, float var5, float var6)
    {
        this.minX = (double)var1;
        this.minY = (double)var2;
        this.minZ = (double)var3;
        this.maxX = (double)var4;
        this.maxY = (double)var5;
        this.maxZ = (double)var6;
    }
    
    // fluid, stairs
    public void velocityToAddToEntity(World world, int var12, int var13, int var14, Entity var3, Vec3D var11)
    {
      
    }

    // leaves, farmland, stairs
    public void onEntityWalking(World var1, int var2, int var3, int var4, Entity var5)
    {

    }

    // Cactus, web, soul sand
    public void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
    {

    }

    internal void setBlockBoundsBasedOnState(World world, int xTile, int yTile, int zTile)
    {
        throw new NotImplementedException();
    }



    public bool renderAsNormalBlock() 
    {
        return true;
    }
    public bool isOpaqueCube()
    {
        return true;
    }
    public bool canCollideCheck(int var1, bool var2)
    {
        return isCollidable();
    }
    public bool isCollidable()
    {
        return true;
    }
    public MovingObjectPosition collisionRayTrace(World var1, int var2, int var3, int var4, Vec3D var5, Vec3D var6)
    {
        this.setBlockBoundsBasedOnState(var1, var2, var3, var4);
        var5 = var5.addVector((double)(-var2), (double)(-var3), (double)(-var4));
        var6 = var6.addVector((double)(-var2), (double)(-var3), (double)(-var4));
        Vec3D var7 = var5.getIntermediateWithXValue(var6, this.minX);
        Vec3D var8 = var5.getIntermediateWithXValue(var6, this.maxX);
        Vec3D var9 = var5.getIntermediateWithYValue(var6, this.minY);
        Vec3D var10 = var5.getIntermediateWithYValue(var6, this.maxY);
        Vec3D var11 = var5.getIntermediateWithZValue(var6, this.minZ);
        Vec3D var12 = var5.getIntermediateWithZValue(var6, this.maxZ);
        if (!this.isVecInsideYZBounds(var7))
        {
            var7 = null;
        }

        if (!this.isVecInsideYZBounds(var8))
        {
            var8 = null;
        }

        if (!this.isVecInsideXZBounds(var9))
        {
            var9 = null;
        }

        if (!this.isVecInsideXZBounds(var10))
        {
            var10 = null;
        }

        if (!this.isVecInsideXYBounds(var11))
        {
            var11 = null;
        }

        if (!this.isVecInsideXYBounds(var12))
        {
            var12 = null;
        }

        Vec3D var13 = null;
        if (var7 != null && (var13 == null || var5.distanceTo(var7) < var5.distanceTo(var13)))
        {
            var13 = var7;
        }

        if (var8 != null && (var13 == null || var5.distanceTo(var8) < var5.distanceTo(var13)))
        {
            var13 = var8;
        }

        if (var9 != null && (var13 == null || var5.distanceTo(var9) < var5.distanceTo(var13)))
        {
            var13 = var9;
        }

        if (var10 != null && (var13 == null || var5.distanceTo(var10) < var5.distanceTo(var13)))
        {
            var13 = var10;
        }

        if (var11 != null && (var13 == null || var5.distanceTo(var11) < var5.distanceTo(var13)))
        {
            var13 = var11;
        }

        if (var12 != null && (var13 == null || var5.distanceTo(var12) < var5.distanceTo(var13)))
        {
            var13 = var12;
        }

        if (var13 == null)
        {
            return null;
        }
        else
        {
            int var14 = -1;
            if (var13 == var7)
            {
                var14 = 4;
            }

            if (var13 == var8)
            {
                var14 = 5;
            }

            if (var13 == var9)
            {
                var14 = 0;
            }

            if (var13 == var10)
            {
                var14 = 1;
            }

            if (var13 == var11)
            {
                var14 = 2;
            }

            if (var13 == var12)
            {
                var14 = 3;
            }

            return new MovingObjectPosition(var2, var3, var4, var14, var13.addVector((double)var2, (double)var3, (double)var4));
        }
    }
    private bool isVecInsideYZBounds(Vec3D var1)
    {
        if (var1 == null)
        {
            return false;
        }
        else
        {
            return var1.yCoord >= this.minY && var1.yCoord <= this.maxY && var1.zCoord >= this.minZ && var1.zCoord <= this.maxZ;
        }
    }

    private bool isVecInsideXZBounds(Vec3D var1)
    {
        if (var1 == null)
        {
            return false;
        }
        else
        {
            return var1.xCoord >= this.minX && var1.xCoord <= this.maxX && var1.zCoord >= this.minZ && var1.zCoord <= this.maxZ;
        }
    }

    private bool isVecInsideXYBounds(Vec3D var1)
    {
        if (var1 == null)
        {
            return false;
        }
        else
        {
            return var1.xCoord >= this.minX && var1.xCoord <= this.maxX && var1.yCoord >= this.minY && var1.yCoord <= this.maxY;
        }
    }
    public Block(int id, Material m)
    {
        blockID = id;
        material = m;
        this.slipperiness = 0.6F;
        if (blocks[id] != null && blocks[id].GetType() == typeof(Block))
        {
            Logger.Error("ID " + id + " is occupied already!");
        }
        else
        {
            this.setBlockBounds(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            blocks[id] = this;
        }

    }

    public int blockID;
    public int blockMeta;
    public string name = "";
    public Material material;
    public bool isSolid = true; //isRenderSolid method in 0.1.3
    public bool isOpaque = true;
    public double minX, minY, minZ, maxX, maxY, maxZ;
    public float slipperiness;
}

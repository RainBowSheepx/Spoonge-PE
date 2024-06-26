﻿using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using System.Collections.Generic;
using System;
using SpoongePE.Core.NBT;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntitySnowball : Entity
    {
        private int xTileSnowball = -1;
        private int yTileSnowball = -1;
        private int zTileSnowball = -1;
        private int inTileSnowball = 0;
        private bool inGroundSnowball = false;
        public int shakeSnowball = 0;
        private EntityLiving thrower;
        private int ticksInGroundSnowball;
        private int ticksInAirSnowball = 0;

        public EntitySnowball(World var1) : base(var1)
        {
            this.setSize(0.25F, 0.25F);
        }



        public bool isInRangeToRenderDist(double var1)
        {
            double var3 = this.boundingBox.getAverageEdgeLength() * 4.0D;
            var3 *= 64.0D;
            return var1 < var3 * var3;
        }

        public EntitySnowball(World var1, EntityLiving var2) : base(var1)
        {
            this.thrower = var2;
            this.setSize(0.25F, 0.25F);
            this.setLocationAndAngles(var2.posX, var2.posY + (double)var2.getEyeHeight(), var2.posZ, var2.rotationYaw, var2.rotationPitch);
            this.posX -= (MathHelper.cos(this.rotationYaw / 180.0F * 3.1415927F) * 0.16F);
            this.posY -= 0.10000000149011612f;
            this.posZ -= (MathHelper.sin(this.rotationYaw / 180.0F * 3.1415927F) * 0.16F);
            this.setPosition(this.posX, this.posY, this.posZ);
            this.yOffset = 0.0F;
            float var3 = 0.4F;
            this.motionX = (-MathHelper.sin(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F) * var3);
            this.motionZ = (MathHelper.cos(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F) * var3);
            this.motionY = (-MathHelper.sin(this.rotationPitch / 180.0F * 3.1415927F) * var3);
            this.setSnowballHeading(this.motionX, this.motionY, this.motionZ, 1.5F, 1.0F);
        }

        public EntitySnowball(World var1, double var2, double var4, double var6) : base(var1)
        {
            
            this.ticksInGroundSnowball = 0;
            this.setSize(0.25F, 0.25F);
            this.setPosition(var2, var4, var6);
            this.yOffset = 0.0F;
        }

        public void setSnowballHeading(double var1, double var3, double var5, float var7, float var8)
        {
            float var9 = MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5);
            var1 /= (double)var9;
            var3 /= (double)var9;
            var5 /= (double)var9;
            var1 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var3 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var5 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var1 *= (double)var7;
            var3 *= (double)var7;
            var5 *= (double)var7;
            this.motionX = (float)var1;
            this.motionY = (float)var3;
            this.motionZ = (float)var5;
            float var10 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
            this.prevRotationYaw = this.rotationYaw = (float)(Math.Atan2(var1, var5) * 180.0D / 3.1415927410125732D);
            this.prevRotationPitch = this.rotationPitch = (float)(Math.Atan2(var3, (double)var10) * 180.0D / 3.1415927410125732D);
            this.ticksInGroundSnowball = 0;
        }

        public new void setVelocity(float var1, float var3, float var5)
        {
            this.motionX = var1;
            this.motionY = var3;
            this.motionZ = var5;
            if (this.prevRotationPitch == 0.0F && this.prevRotationYaw == 0.0F)
            {
                float var7 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
                this.prevRotationYaw = this.rotationYaw = (float)(Math.Atan2(var1, var5) * 180.0D / 3.1415927410125732D);
                this.prevRotationPitch = this.rotationPitch = (float)(Math.Atan2(var3, (double)var7) * 180.0D / 3.1415927410125732D);
            }

        }

        public new void onUpdate()
        {
            this.lastTickPosX = this.posX;
            this.lastTickPosY = this.posY;
            this.lastTickPosZ = this.posZ;
            base.onUpdate();
            if (this.shakeSnowball > 0)
            {
                --this.shakeSnowball;
            }

            if (this.inGroundSnowball)
            {
                int var1 = this.world.getBlockIDAt(this.xTileSnowball, this.yTileSnowball, this.zTileSnowball);
                if (var1 == this.inTileSnowball)
                {
                    ++this.ticksInGroundSnowball;
                    if (this.ticksInGroundSnowball == 1200)
                    {
                        this.setEntityDead();
                    }

                    return;
                }

                this.inGroundSnowball = false;
                this.motionX *= (this.rand.NextSingle() * 0.2F);
                this.motionY *= (this.rand.NextSingle() * 0.2F);
                this.motionZ *= (this.rand.NextSingle() * 0.2F);
                this.ticksInGroundSnowball = 0;
                this.ticksInAirSnowball = 0;
            }
            else
            {
                ++this.ticksInAirSnowball;
            }

            Vec3D var15 = Vec3D.createVector(this.posX, this.posY, this.posZ);
            Vec3D var2 = Vec3D.createVector(this.posX + this.motionX, this.posY + this.motionY, this.posZ + this.motionZ);
            MovingObjectPosition var3 = this.world.rayTraceBlocks(var15, var2);
            var15 = Vec3D.createVector(this.posX, this.posY, this.posZ);
            var2 = Vec3D.createVector(this.posX + this.motionX, this.posY + this.motionY, this.posZ + this.motionZ);
            if (var3 != null)
            {
                var2 = Vec3D.createVector(var3.hitVec.xCoord, var3.hitVec.yCoord, var3.hitVec.zCoord);
            }


                Entity var4 = null;
                List<Entity> var5 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox.addCoord(this.motionX, this.motionY, this.motionZ).expand(1.0D, 1.0D, 1.0D));
                double var6 = 0.0D;

                for (int var8 = 0; var8 < var5.Count; ++var8)
                {
                    Entity var9 = (Entity)var5[var8];
                    if (var9.canBeCollidedWith() && (var9 != this.thrower || this.ticksInAirSnowball >= 5))
                    {
                        float var10 = 0.3F;
                        AxisAlignedBB var11 = var9.boundingBox.expand((double)var10, (double)var10, (double)var10);
                        MovingObjectPosition var12 = var11.calculateIntercept(var15, var2);
                        if (var12 != null)
                        {
                            double var13 = var15.distanceTo(var12.hitVec);
                            if (var13 < var6 || var6 == 0.0D)
                            {
                                var4 = var9;
                                var6 = var13;
                            }
                        }
                    }
                }

                if (var4 != null)
                {
                    var3 = new MovingObjectPosition(var4);
                }
            

            if (var3 != null)
            {
                if (var3.entityHit != null && var3.entityHit.attackEntityFrom(this.thrower, 0))
                {
                }



                this.setEntityDead();
            }

            this.posX += this.motionX;
            this.posY += this.motionY;
            this.posZ += this.motionZ;
            float var17 = MathHelper.sqrt_double(this.motionX * this.motionX + this.motionZ * this.motionZ);
            this.rotationYaw = (float)(Math.Atan2(this.motionX, this.motionZ) * 180.0D / 3.1415927410125732D);

            for (this.rotationPitch = (float)(Math.Atan2(this.motionY, (double)var17) * 180.0D / 3.1415927410125732D); this.rotationPitch - this.prevRotationPitch < -180.0F; this.prevRotationPitch -= 360.0F)
            {
            }

            while (this.rotationPitch - this.prevRotationPitch >= 180.0F)
            {
                this.prevRotationPitch += 360.0F;
            }

            while (this.rotationYaw - this.prevRotationYaw < -180.0F)
            {
                this.prevRotationYaw -= 360.0F;
            }

            while (this.rotationYaw - this.prevRotationYaw >= 180.0F)
            {
                this.prevRotationYaw += 360.0F;
            }

            this.rotationPitch = this.prevRotationPitch + (this.rotationPitch - this.prevRotationPitch) * 0.2F;
            this.rotationYaw = this.prevRotationYaw + (this.rotationYaw - this.prevRotationYaw) * 0.2F;
            float var18 = 0.99F;
            float var19 = 0.03F;
            if (this.isInWater())
            {


                var18 = 0.8F;
            }

            this.motionX *= var18;
            this.motionY *= var18;
            this.motionZ *= var18;
            this.motionY -= var19;
            this.setPosition(this.posX, this.posY, this.posZ);
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtShort("xTile", (short)this.xTileSnowball));
            var1.Add(new NbtShort("yTile", (short)this.yTileSnowball));
            var1.Add(new NbtShort("zTile", (short)this.zTileSnowball));
            var1.Add(new NbtByte("inTile", (byte)this.inTileSnowball));
            var1.Add(new NbtByte("shake", (byte)this.shakeSnowball));
            var1.Add(new NbtByte("inGround", (byte)(this.inGroundSnowball ? 1 : 0)));
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            this.xTileSnowball = var1.Get<NbtShort>("xTile").ShortValue;
            this.yTileSnowball = var1.Get<NbtShort>("yTile").ShortValue;
            this.zTileSnowball = var1.Get<NbtShort>("zTile").ShortValue;
            this.inTileSnowball = var1.Get<NbtByte>("inTile").ByteValue & 255;
            this.shakeSnowball = var1.Get<NbtByte>("shake").ByteValue & 255;
            this.inGroundSnowball = var1.Get<NbtByte>("inGround").ByteValue == 1;
        }

        public new void onCollideWithPlayer(EntityPlayer var1)
        {
            if (this.inGroundSnowball && this.thrower == var1 && this.shakeSnowball <= 0 && var1.inventory.addItemStackToInventory(new ItemStack(Item.arrow, 1)))
            {
          
                var1.onItemPickup(this, 1);
                this.setEntityDead();
            }

        }

        public new float getShadowSize()
        {
            return 0.0F;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class TickNextTickData : IComparable<TickNextTickData>
    {
        private static long nextID = 0;
        public int posX, posY, posZ, blockID;
        public long scheduledTime;
        private long tickEntryID;
        public TickNextTickData(int x, int y, int z, int id)
        {
            this.tickEntryID = ++TickNextTickData.nextID;
            this.posX = x;
            this.posY = y;
            this.posZ = z;
            this.blockID = id;
        }

        public int hashCode()
        {
            return this.blockID + ((this.posY + ((this.posZ + (this.posX << 0xA)) << 7)) << 8);
        }

        public bool equals(Object o)
        {
            if (o.GetType() == typeof(TickNextTickData)) {
                TickNextTickData tick = (TickNextTickData)o;
                return this.posX == tick.posX && this.posY == tick.posY && this.posZ == tick.posZ && this.blockID == tick.blockID;
            }
            return false;
        }

        public new string ToString => this.tickEntryID + ": " + posX + "/" + posY + "/" + posZ + "/" + blockID + "/" + scheduledTime;

        public int CompareTo(TickNextTickData tick)
        {
            if (this.scheduledTime < tick.scheduledTime) return -1;
            else if (this.scheduledTime > tick.scheduledTime) return 1;
            else if (this.tickEntryID < tick.tickEntryID) return -1;
            return this.tickEntryID <= tick.tickEntryID ? 0 : 1;
        }
    }
}

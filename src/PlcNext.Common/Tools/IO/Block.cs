#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Tools.IO
{
    internal class Block
    {
        public long Offset { get; set; }

        public long Capacity { get; set; }

        public bool IsAllocated { get; set; }

        public Block Lower { get; set; }

        public Block Higher { get; set; }

        public Block Companion { get; set; }

        public Block Composite { get; set; }

        public Block Allocate(long requestedCapacity)
        {
            if (requestedCapacity <= Capacity)
            {
                if (IsAllocated)
                {
                    return SearchThroughCompanionBuddies(requestedCapacity);
                }
                else
                {
                    long halfOfBlockCapacity = Capacity / 2;

                    if (requestedCapacity <= halfOfBlockCapacity)
                    {
                        return SplitBlockIntoTwoEqualCompanionBuddies(requestedCapacity, halfOfBlockCapacity);
                    }
                    else
                    {
                        IsAllocated = true;
                        return this;
                    }
                }
            }
            return null;
        }

        public void Release()
        {
            IsAllocated = false;
            if (Companion != null)
            {
                if (!Companion.IsAllocated && Composite != null)
                {
                    Composite.Release();
                }
            }
        }

        private Block SplitBlockIntoTwoEqualCompanionBuddies(long requestedCapacity, long halfCapacity)
        {
            Lower = new Block
            {
                Offset = Offset,
                Capacity = halfCapacity,
                Composite = this
            };

            Higher = new Block
            {
                Offset = Offset + halfCapacity,
                Capacity = halfCapacity,
                Composite = this
            };

            Lower.Companion = Higher;
            Higher.Companion = Lower;

            IsAllocated = true;

            return Lower.Allocate(requestedCapacity);
        }

        private Block SearchThroughCompanionBuddies(long requestedCapacity)
        {
            if (Lower != null)
            {
                Block lowerResult = Lower.Allocate(requestedCapacity);

                if (lowerResult != null)
                {
                    return lowerResult;
                }
            }

            if (Higher != null)
            {
                Block higherResult = Higher.Allocate(requestedCapacity);

                if (higherResult != null)
                {
                    return higherResult;
                }
            }

            return null;
        }
    }
}
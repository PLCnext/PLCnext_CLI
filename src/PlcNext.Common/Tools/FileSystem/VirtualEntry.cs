#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.FileSystem
{
    public abstract class VirtualEntry
    {
        private readonly IEntryContentResolver entryContentResolver;

        protected VirtualEntry(string name, IEntryContentResolver entryContentResolver)
        {
            Name = name;
            this.entryContentResolver = entryContentResolver;
        }

        public VirtualDirectory Parent { get; private set; }

        internal void SetParent(VirtualDirectory parent)
        {
            Parent = parent;
        }

        public string Name { get; }

        public string FullName => entryContentResolver.FullName;

        public bool Deleted { get; private set; }

        public virtual void Delete()
        {
            entryContentResolver.Delete();
            Parent?.RemoveEntry(this);
            Deleted = true;
        }

        public virtual void UnDelete(bool checkDeleted = true)
        {
            if (checkDeleted && !Deleted)
            {
                throw new InvalidOperationException("Cannot undelete a non deleted entry.");
            }
            entryContentResolver.UnDelete();
            Parent.AddEntry(this);
            Deleted = false;
        }

        public virtual void Restore()
        {
            if (entryContentResolver.Created)
            {
                Delete();
            }
            else
            {
                UnDelete(false);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
        
        protected bool Equals(VirtualEntry other)
        {
            return other != null && string.Equals(FullName, other.FullName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VirtualEntry)obj);
        }

        public override int GetHashCode()
        {
            int result = (FullName != null ? FullName.GetHashCode() : 0);
            return result;
        }

        public static bool operator ==(VirtualEntry left, VirtualEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VirtualEntry left, VirtualEntry right)
        {
            return !Equals(left, right);
        }
    }
}

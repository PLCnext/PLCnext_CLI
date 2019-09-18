#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.DataModel
{
    internal class CollectiveEntityContentProvider : PriorityContentProvider
    {
        private readonly IEnumerable<IEntityContentProvider> contentResolvers;
        private CycleChecker<Resolution> cycleChecker;
        private CycleChecker<Resolution> canCycleChecker;

        public CollectiveEntityContentProvider(IEnumerable<IEntityContentProvider> contentResolvers, IPriorityMaster priorityMaster)
        {
            this.contentResolvers = priorityMaster.SortPriorities(contentResolvers.Cast<PriorityContentProvider>());
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            if (cycleChecker?.HasItem(new Resolution(owner, key)) == true)
            {
                //break cycles before they form
                return false;
            }
            using (canCycleChecker = canCycleChecker?.SpawnChild() ?? new CycleChecker<Resolution>(ExceptionTexts.ContentProviderCycle,
                                                                                             () => canCycleChecker = null))
            {
                Resolution resolution = new Resolution(owner, key);
                canCycleChecker.AddItem(resolution);
                bool result = contentResolvers.Any(r => r.CanResolve(owner, key, false));
                if (!result)
                {
                    canCycleChecker.RemoveAfter(resolution);
                    result = contentResolvers.Any(r => r.CanResolve(owner, key, true));
                }

                return result;
            }
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            using (cycleChecker = cycleChecker?.SpawnChild()?? new CycleChecker<Resolution>(ExceptionTexts.ContentProviderCycle,
                                                                                            () => cycleChecker = null))
            {
                fallback = false;
                IEntityContentProvider provider = GetContentProvider();
                if (provider == null)
                {
                    fallback = true;
                    provider = GetContentProvider();
                }
                if (provider == null)
                {
                    throw new ContentProviderException(key, owner);
                }

                cycleChecker.AddItem(new Resolution(owner, key, provider));
                return provider.Resolve(owner, key, fallback);
            }

            IEntityContentProvider GetContentProvider()
            {
                return contentResolvers.Where(r => !cycleChecker.HasItem(new Resolution(owner, key, r)))
                                       .FirstOrDefault(r => r.CanResolve(owner, key, fallback))
                       ?? contentResolvers.FirstOrDefault(r => r.CanResolve(owner, key, fallback));
            }
        }

        private class Resolution : IEquatable<Resolution>
        {
            private readonly Entity entity;
            private readonly string key;
            private readonly IEntityContentProvider entityContentProvider;

            public Resolution(Entity entity, string key, IEntityContentProvider entityContentProvider)
            {
                this.entity = entity;
                this.key = key;
                this.entityContentProvider = entityContentProvider;
            }

            public Resolution(Entity entity, string key)
            {
                this.entity = entity;
                this.key = key;
            }

            public override string ToString()
            {
                return $"{nameof(entity)}: {entity}, {nameof(key)}: {key}, {nameof(entityContentProvider)}: {entityContentProvider?.GetType().Name}";
            }

            public bool Equals(Resolution other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return Equals(entity, other.entity) && key == other.key && 
                       (Equals(entityContentProvider, other.entityContentProvider) ||
                        entityContentProvider == null ||
                        other.entityContentProvider == null);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((Resolution) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (entity != null ? entity.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (key != null ? key.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(Resolution left, Resolution right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Resolution left, Resolution right)
            {
                return !Equals(left, right);
            }
        }
    }
}

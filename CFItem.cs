﻿using System;
using CompoundFileStorage.Exceptions;
using CompoundFileStorage.Interfaces;

/*
     The contents of this file are subject to the Mozilla Public License
     Version 1.1 (the "License"); you may not use this file except in
     compliance with the License. You may obtain a copy of the License at
     http://www.mozilla.org/MPL/

     Software distributed under the License is distributed on an "AS IS"
     basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
     License for the specific language governing rights and limitations
     under the License.

     The Original Code is OpenMCDF - Compound Document Format library.

     The Initial Developer of the Original Code is Federico Blaseotto.
 
     The code is modified to more now a days standards and upgraded to
     C# .NET 4.0 by Kees van Spelde
*/

namespace CompoundFileStorage
{
    /// <summary>
    ///     Abstract base class for Structured Storage entities.
    /// </summary>
    public abstract class CFItem : IComparable, ICFItem
    {
        #region Fields
        private readonly CompoundFile _compoundFile;
        #endregion

        #region Properties
        /// <summary>
        /// The <see cref="CompoundFile"/>
        /// </summary>
        protected CompoundFile CompoundFile
        {
            get { return _compoundFile; }
        }

        /// <summary>
        ///     Get entity name
        /// </summary>
        public string Name
        {
            get
            {
                var name = DirEntry.GetEntryName();
                return !string.IsNullOrEmpty(name) ? name.TrimEnd('\0') : string.Empty;
            }
        }

        /// <summary>
        ///     Size in bytes of the item. It has a valid value
        ///     only if entity is a stream, otherwise it is setted to zero.
        /// </summary>
        public long Size
        {
            get { return DirEntry.Size; }
        }


        /// <summary>
        ///     Returns true if item is Storage
        /// </summary>
        /// <remarks>
        ///     This check doesn't use reflection or runtime type information
        ///     and doesn't suffer related performance penalties.
        /// </remarks>
        public bool IsStorage
        {
            get { return DirEntry.StgType == StgType.StgStorage; }
        }

        /// <summary>
        ///     Returns true if item is a Stream
        /// </summary>
        /// <remarks>
        ///     This check doesn't use reflection or runtime type information
        ///     and doesn't suffer related performance penalties.
        /// </remarks>
        public bool IsStream
        {
            get { return DirEntry.StgType == StgType.StgStream; }
        }

        /// <summary>
        ///     Returnstrue if item is the Root Storage
        /// </summary>
        /// <remarks>
        ///     This check doesn't use reflection or runtime type information
        ///     and doesn't suffer related performance penalties.
        /// </remarks>
        public bool IsRoot
        {
            get { return DirEntry.StgType == StgType.StgRoot; }
        }

        /// <summary>
        ///     Get/Set the Creation Date of the current item
        /// </summary>
        public DateTime CreationDate
        {
            get { return DateTime.FromFileTime(BitConverter.ToInt64(DirEntry.CreationDate, 0)); }

            set
            {
                if (DirEntry.StgType != StgType.StgStream && DirEntry.StgType != StgType.StgRoot)
                    DirEntry.CreationDate = BitConverter.GetBytes((value.ToFileTime()));
                else
                    throw new CFException("CreationDate can only be set on storage entries");
            }
        }

        /// <summary>
        ///     Get/Set the Modify Date of the current item
        /// </summary>
        public DateTime ModifyDate
        {
            get { return DateTime.FromFileTime(BitConverter.ToInt64(DirEntry.ModifyDate, 0)); }

            set
            {
                if (DirEntry.StgType != StgType.StgStream && DirEntry.StgType != StgType.StgRoot)
                    DirEntry.ModifyDate = BitConverter.GetBytes((value.ToFileTime()));
                else
                    throw new CFException("ModifyDate can only be set on storage entries");
            }
        }

        /// <summary>
        ///     Get/Set Object class Guid for Root and Storage entries.
        /// </summary>
        public Guid CLSID
        {
            get { return DirEntry.StorageCLSID; }
            set
            {
                if (DirEntry.StgType != StgType.StgStream)
                    DirEntry.StorageCLSID = value;
                else
                    throw new CFException("Object class GUID can only be set on Root and Storage entries");
            }
        }
        #endregion

        #region CheckDisposed
        internal void CheckDisposed()
        {
            if (_compoundFile.IsClosed)
                throw new CFDisposedException(
                    "The owner compound file has been closed and owned items have been invalidated");
        }
        #endregion

        #region CFItem
        /// <summary>
        /// Creates a new CFItem object
        /// </summary>
        protected CFItem()
        {
        }

        /// <summary>
        /// Creates a new CFItem object and sets the <paramref name="compoundFile"/>
        /// </summary>
        /// <param name="compoundFile"></param>
        protected CFItem(CompoundFile compoundFile)
        {
            _compoundFile = compoundFile;
        }
        #endregion

        #region IDirectoryEntry Members
        internal IDirectoryEntry DirEntry { get; set; }

        internal int CompareTo(CFItem other)
        {
            return DirEntry.CompareTo(other.DirEntry);
        }
        #endregion

        #region IComparable Members
        public int CompareTo(object obj)
        {
            return DirEntry.CompareTo(((CFItem) obj).DirEntry);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Returns true when the <paramref name="leftItem"/> is the same as the <paramref name="rightItem"/>
        /// </summary>
        /// <param name="leftItem"></param>
        /// <param name="rightItem"></param>
        /// <returns></returns>
        public static bool operator ==(CFItem leftItem, CFItem rightItem)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(leftItem, rightItem))
                return true;

            // If one is null, but not both, return false.
            if (((object) leftItem == null) || ((object) rightItem == null))
                return false;

            // Return true if the fields match:
            return leftItem.CompareTo(rightItem) == 0;
        }

        /// <summary>
        /// Returns true when the <paramref name="leftItem"/> is not the same as the <paramref name="rightItem"/>
        /// </summary>
        /// <param name="leftItem"></param>
        /// <param name="rightItem"></param>
        /// <returns></returns>
        public static bool operator !=(CFItem leftItem, CFItem rightItem)
        {
            return !(leftItem == rightItem);
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        /// Returns the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return DirEntry.GetEntryName().GetHashCode();
        }
        #endregion
    }
}
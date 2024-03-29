//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace netdomain.LinqToEntities.Test.BusinessObjects
{
    public partial class PersonPoco
    {
        #region Primitive Properties
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual string Beruf
        {
            get;
            set;
        }
    
        public virtual byte[] Version
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> NVersion
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<AdressePoco> Adressliste
        {
            get
            {
                if (_adressliste == null)
                {
                    var newCollection = new FixupCollection<AdressePoco>();
                    newCollection.CollectionChanged += FixupAdressliste;
                    _adressliste = newCollection;
                }
                return _adressliste;
            }
            set
            {
                if (!ReferenceEquals(_adressliste, value))
                {
                    var previousValue = _adressliste as FixupCollection<AdressePoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAdressliste;
                    }
                    _adressliste = value;
                    var newValue = value as FixupCollection<AdressePoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAdressliste;
                    }
                }
            }
        }
        private ICollection<AdressePoco> _adressliste;

        #endregion
        #region Association Fixup
    
        private void FixupAdressliste(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AdressePoco item in e.NewItems)
                {
                    item.Person = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AdressePoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Person, this))
                    {
                        item.Person = null;
                    }
                }
            }
        }

        #endregion
    }
}

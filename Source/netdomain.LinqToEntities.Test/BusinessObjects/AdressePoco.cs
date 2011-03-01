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
    public partial class AdressePoco
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
    
        public virtual PersonPoco Person
        {
            get { return _person; }
            set
            {
                if (!ReferenceEquals(_person, value))
                {
                    var previousValue = _person;
                    _person = value;
                    FixupPerson(previousValue);
                }
            }
        }
        private PersonPoco _person;
    
        public virtual ICollection<AdresseDetailPoco> AdresseDetails
        {
            get
            {
                if (_adresseDetails == null)
                {
                    _adresseDetails = new FixupCollection<AdresseDetailPoco>();
                }
                return _adresseDetails;
            }
            set
            {
                _adresseDetails = value;
            }
        }
        private ICollection<AdresseDetailPoco> _adresseDetails;

        #endregion
        #region Association Fixup
    
        private void FixupPerson(PersonPoco previousValue)
        {
            if (previousValue != null && previousValue.Adressliste.Contains(this))
            {
                previousValue.Adressliste.Remove(this);
            }
    
            if (Person != null)
            {
                if (!Person.Adressliste.Contains(this))
                {
                    Person.Adressliste.Add(this);
                }
            }
        }

        #endregion
    }
}

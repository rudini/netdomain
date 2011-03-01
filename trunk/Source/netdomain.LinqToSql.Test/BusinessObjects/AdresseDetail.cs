//-------------------------------------------------------------------------------
// <copyright file="AdresseDetail.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Roger Rudin, bbv Software Services AG
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.LinqToSql.Test.BusinessObjects
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implements an address entity.
    /// </summary>
    [DataContract]
    public class AdresseDetail : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int id;

        private int? adresseId;

        private EntityRef<Adresse> adresse;

        public AdresseDetail()
        {
            this.Initialize();
        }

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if ((this.id != value))
                {
                    this.SendPropertyChanging();
                    this.id = value;
                    this.SendPropertyChanged("Id");
                }
            }
        }

        public int? AdresseId
        {
            get
            {
                return this.adresseId;
            }
            set
            {
                if ((this.adresseId != value))
                {
                    if (this.adresse.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this.adresseId = value;
                    this.SendPropertyChanged("AdresseId");
                }
            }
        }

        public Adresse Adresse
        {
            get
            {
                return this.adresse.Entity;
            }
            set
            {
                Adresse previousValue = this.adresse.Entity;
                if (((previousValue != value)
                            || (this.adresse.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this.adresse.Entity = null;
                        previousValue.AdresseDetails.Remove(this);
                    }
                    this.adresse.Entity = value;
                    if ((value != null))
                    {
                        value.AdresseDetails.Add(this);
                        this.adresseId = value.Id;
                    }
                    else
                    {
                        this.adresseId = default(Nullable<int>);
                    }
                    this.SendPropertyChanged("Adresse");
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.adresse = default(EntityRef<Adresse>);
        }
    }
}


//-------------------------------------------------------------------------------
// <copyright file="Adresse.cs" company="bbv Software Services AG">
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
    using Abstract;
    using FluentValidation;
    using FluentValidation.Results;

    /// <summary>
    /// Implements an address entity.
    /// </summary>
    [DataContract]
    public class Adresse : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static readonly PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int id;

        private byte[] version;

        private string name;

        private int? personId;

        private EntityRef<Person> person;

        /// <summary>
        /// Initializes a new instance of the <see cref="Adresse"/> class.
        /// </summary>
        public Adresse()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember(Order = 1)]
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Order = 2)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if ((this.name != value))
                {
                    this.SendPropertyChanging();
                    this.name = value;
                    this.SendPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the person Id.
        /// </summary>
        /// <value>The person ID.</value>
        [DataMember(Order = 3)]
        public int? PersonId
        {
            get
            {
                return this.personId;
            }
            set
            {
                if ((this.personId != value))
                {
                    if (this.person.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.SendPropertyChanging();
                    this.personId = value;
                    this.SendPropertyChanged("PersonID");
                }
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DataMember(Order = 4)]
        public byte[] Version
        {
            get { return this.version; }
            set
            {
                if ((this.version != value))
                {
                    this.SendPropertyChanging();
                    this.version = value;
                    this.SendPropertyChanged("Version");
                }
            }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        /// <value>The person.</value>
        [DataMember(Order = 5)]
        public Person Person
        {
            get
            {
                return this.person.Entity;
            }
            set
            {
                Person previousValue = this.person.Entity;
                if (((previousValue != value)
                            || (this.person.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this.person.Entity = null;
                        previousValue.Adressliste.Remove(this);
                    }
                    this.person.Entity = value;
                    if ((value != null))
                    {
                        value.Adressliste.Add(this);
                        this.personId = value.Id;
                    }
                    else
                    {
                        this.personId = default(Nullable<int>);
                    }
                    this.SendPropertyChanged("Person");
                }
            }
        }

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sends the property changing.
        /// </summary>
        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        /// <summary>
        /// Sends the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
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
            this.person = default(EntityRef<Person>);
        }

        /// <summary>
        /// Called when [deserializing].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Validates the specified results.
        /// </summary>
        /// <param name="result">The results.</param>
        public void ValidateRegardingPersistence(ValidationResult result)
        {
            var validationResults = new InlineValidator<Adresse>
                          {
                              v => v.RuleFor(e => e.Name).Length(1, 20).WithMessage("The length of the field Name must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error),
                          }.Validate(this);

            foreach (var validationResult in validationResults.Errors)
            {
                result.Errors.Add(validationResult);
            }
        }
    }
}

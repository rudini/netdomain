//-------------------------------------------------------------------------------
// <copyright file="Person.cs" company="bbv Software Services AG">
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

    [DataContract]
    public class Person : INotifyPropertyChanging, INotifyPropertyChanged, IValidatable<ValidationResult>
    {
        private static readonly PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int id;

        private byte[] version;

        private string name;

        private string beruf;

        private EntitySet<Adresse> adressliste;

        private bool serializing;

        /// <summary>
        /// Initializes a new instance of the <see cref="Person"/> class.
        /// </summary>
        public Person()
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
        /// Gets or sets the beruf.
        /// </summary>
        /// <value>The beruf.</value>
        [DataMember(Order = 3)]
        public string Beruf
        {
            get
            {
                return this.beruf;
            }
            set
            {
                if ((this.beruf != value))
                {
                    this.SendPropertyChanging();
                    this.beruf = value;
                    this.SendPropertyChanged("Beruf");
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
        /// Gets or sets the adressliste.
        /// </summary>
        /// <value>The adressliste.</value>
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public EntitySet<Adresse> Adressliste
        {
            get
            {
                if ((this.serializing
                            && (this.adressliste.HasLoadedOrAssignedValues == false)))
                {
                    return null;
                }
                return this.adressliste;
            }
            set
            {
                this.adressliste.Assign(value);
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
        /// Attaches the adresses.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void AttachAdresses(Adresse entity)
        {
            this.SendPropertyChanging();
            entity.Person = this;
        }

        /// <summary>
        /// Detaches the adresses.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void DetachAdresses(Adresse entity)
        {
            this.SendPropertyChanging();
            entity.Person = null;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.adressliste = new EntitySet<Adresse>(this.AttachAdresses, this.DetachAdresses);
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
        /// Called when [serializing].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnSerializing]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void OnSerializing(StreamingContext context)
        {
            this.serializing = true;
        }

        /// <summary>
        /// Called when [serialized].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnSerialized]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void OnSerialized(StreamingContext context)
        {
            this.serializing = false;
        }

        /// <summary>
        /// Validates this instance and all aggregated entities and returns the validation results.
        /// </summary>
        /// <param name="results">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        /// <param name="context"></param>
        public void Validate(ValidationResult results, object context = null)
        {
            // context based validation
        }

        /// <summary>
        /// Validates this instance and all aggregated entities regarding persistence validation rules and returns the validation results.
        /// </summary>
        /// <param name="result">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        public void ValidateRegardingPersistence(ValidationResult result)
        {
            var validationResults = new InlineValidator<Person>
                          {
                              v => v.RuleFor(e => e.Name).Length(1, 10).WithMessage("The length of the field Name must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error),
                              v => v.RuleFor(e => e.Beruf).Length(1, 20).WithMessage("The length of the field Beruf must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error)
                          }.Validate(this);

            foreach (var validationResult in validationResults.Errors)
            {
                result.Errors.Add(validationResult);
            }

            foreach (var address in this.Adressliste)
            {
                address.ValidateRegardingPersistence(result);
            }
        }
    }
}

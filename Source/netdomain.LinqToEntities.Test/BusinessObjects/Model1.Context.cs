//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.EntityClient;

namespace netdomain.LinqToEntities.Test.BusinessObjects
{
    public partial class LinqTestEntities : ObjectContext
    {
        public const string ConnectionString = "name=LinqTestEntities";
        public const string ContainerName = "LinqTestEntities";
    
        #region Constructors
    
        public LinqTestEntities()
            : base(ConnectionString, ContainerName)
        {
        }
    
        public LinqTestEntities(string connectionString)
            : base(connectionString, ContainerName)
        {
        }
    
        public LinqTestEntities(EntityConnection connection)
            : base(connection, ContainerName)
        {
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<AdressePoco> Adresse
        {
            get { return _adresse  ?? (_adresse = CreateObjectSet<AdressePoco>("Adresse")); }
        }
        private ObjectSet<AdressePoco> _adresse;
    
        public ObjectSet<PersonPoco> Person
        {
            get { return _person  ?? (_person = CreateObjectSet<PersonPoco>("Person")); }
        }
        private ObjectSet<PersonPoco> _person;

        #endregion
    }
}

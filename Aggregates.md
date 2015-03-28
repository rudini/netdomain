# Introduction #

netdomain supports lambda expression to define the aggregate.


# Details #

LinqToSql and LinqToEntities (EF) now supports the using of lambda expression on the Include method.

Here is an example how to eager load all entities of the Northwind database in one query.

```
        [Test]
        public void QueryAnObjectGraph_UsingInclude_MustReturnAllObjects()
        {
            // Arrange
            var context = new NORTHWNDEntities { ContextOptions = { LazyLoadingEnabled = false } };
            IWorkspace ws = new LinqToEntitiesWorkspace(context);
            // var context = new DataClasses1DataContext { DeferredLoadingEnabled = false };
            // IWorkspace ws = new LinqToSqlWorkspace(context);

            // Act
            var query = ws.CreateQuery<Order>()
                 .Include(o => o.Employee)
                 .Include(o => o.Shipper)
                 .Include(o => o.Order_Details.Select(det => det.Product).Select(p => p.Category))
                 .Include(o => o.Order_Details.Select(det => det.Product).Select(p => p.Supplier));
            var order = query.Where(o => o.OrderID == 10255).First();

            // Assert
            Assert.IsNotNull(order);
            Assert.IsTrue(order.Order_Details.Any());
            Assert.IsNotNull(order.Employee);
            Assert.IsNotNull(order.Shipper);
            Assert.IsNotNull(order.Order_Details.First().Product);
            Assert.IsNotNull(order.Order_Details.First().Product.Category);
            Assert.IsNotNull(order.Order_Details.First().Product.Supplier);
        }
```

NHibernate uses its own syntax to eager load data. All .Fetch and .FetchMany methods must be called at the end of the linq query.

```
        var fetchedPerson2 =
                    this.Testee.CreateQuery<Person>()
                        .Where(p => p.Name == name)
                        .FetchMany(p => p.Adressliste)
                        .ThenFetch(a => a.AdresseDetails).First();
```
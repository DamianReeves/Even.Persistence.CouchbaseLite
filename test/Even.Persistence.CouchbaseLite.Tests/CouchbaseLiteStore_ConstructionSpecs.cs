using System;
using Akka.TestKit.Xunit2;
using Couchbase.Lite;
using Even.Persistence.CouchbaseLite.Internals;
using Even.Persistence.CouchbaseLite.Testing;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Even.Persistence.CouchbaseLite
{
    public class CouchbaseLiteStore_ConstructionSpecs : TestKit
    {
        [Fact]
        public void When_constructing_actorSystem_is_required()
        {

            using (var db = TestingUtility.GetDatabaseForCaller())
            {
                Assert
                        .Throws<ArgumentNullException>(() => new CouchbaseLiteStore(null, db, Substitute.For<ISequenceGenerator>()))
                        .ParamName.Should().Be("actorSystem");
            }                
        }

        [Fact]
        public void When_constructing_passed_in_actorSystem_should_be_assigned_to_Sys_property()
        {
            using (var db = TestingUtility.GetDatabaseForCaller())
            {
                var store = new CouchbaseLiteStore(Sys, db, Substitute.For<ISequenceGenerator>());
                store.Sys.Should().BeSameAs(Sys);
            }                
        }

        [Fact]
        public void When_constructing_database_is_required()
        {
            Assert
                .Throws<ArgumentNullException>(() => new CouchbaseLiteStore(Sys, null, Substitute.For<ISequenceGenerator>()))
                .ParamName.Should().Be("db");
        }

        [Fact]
        public void When_constructing_passed_in_Db_should_be_Db_used()
        {
            using (var db = TestingUtility.GetDatabaseForCaller())
            {
                var store = new CouchbaseLiteStore(Sys, db, Substitute.For<ISequenceGenerator>());
                store.Db.Should().BeSameAs(db);
            }                
        }
    }
}

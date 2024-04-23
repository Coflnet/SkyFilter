using System.Linq;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Data.Common;
using Moq;

namespace Coflnet.Sky.Filter
{
    public class EnchantmentTests
    {
        //[Test]
        public void EfficientQuery()
        {
            var filters = new FilterEngine();
            var con = new Moq.Mock<DbConnection>();
            //con.Setup(c=>c.CreateCommand()).Returns(new Mock<DbCommand>().Object);
            var options = new DbContextOptionsBuilder<TestContext>()
            //.UseInMemoryDatabase(databaseName: "AuctionsDb")
            .UseMySql("server=mariadxb;user=root;password=takenfrombitnami;database=test", new MariaDbServerVersion("10.3"))
            .Options;

            // Insert seed data into the database using one instance of the context
            using var context = new TestContext(options);


            var full = filters.AddFilters(context.Auctions, new System.Collections.Generic.Dictionary<string, string>()
            {
                { "Enchantment", "sharpness"},
                { "EnchantLvl", "4"},
                {"ItemId","5"}
            }, true);
            Assert.That(full.ToQueryString().Contains("MinimumAuctionId"));
        }

        [Test]
        public void LevelRange()
        {
            var filters = new FilterEngine();


            var full = filters.GetMatcher(new System.Collections.Generic.Dictionary<string, string>()
            {
                { "Enchantment", "corruption"},
                { "EnchantLvl", "1-5"}
            });
            Assert.That(full(new SaveAuction() { Enchantments = new() { new Enchantment(Enchantment.EnchantmentType.corruption, 1) } }));
            Assert.That(full(new SaveAuction() { Enchantments = new() { new Enchantment(Enchantment.EnchantmentType.corruption, 5) } }));
        }

        [Test]
        public void Empty()
        {
            var filters = new FilterEngine();

            Assert.Throws<CoflnetException>(() => filters.GetMatcher(new System.Collections.Generic.Dictionary<string, string>()
            {
                { "Enchantment", ""},
                { "EnchantLvl", ""}
            }), "The value `` is not a known enchant");
        }

        [Test]
        public void NoOtherValuableEnchant()
        {
            var filters = new FilterEngine();

            var full = filters.GetMatcher(new System.Collections.Generic.Dictionary<string, string>()
            {
                { "NoOtherValuableEnchants", "true"},
                { "sharpness", "7"}
            });
            var match = new Enchantment(Enchantment.EnchantmentType.sharpness, 7);
            var nomatch = new Enchantment(Enchantment.EnchantmentType.growth, 7);
            var ignored = new Enchantment(Enchantment.EnchantmentType.protection, 5);
            Assert.That(full(new () { Enchantments = new() { match } }));
            Assert.That(!full(new () { Enchantments = new() { nomatch } }));
            Assert.That(!full(new () { Enchantments = new() { match, nomatch } }));
            Assert.That(full(new () { Enchantments = new() { match, ignored } }));
        }
    }

    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SaveAuction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<SaveAuction>(entity =>
            {
                entity.HasIndex(e => e.End);
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => new { e.ItemId, e.End });
                entity.HasMany(e => e.NBTLookup).WithOne().HasForeignKey("AuctionId");
                entity.HasIndex(e => e.UId).IsUnique();
                //entity.HasOne<NbtData>(d=>d.NbtData);
                //entity.HasMany<Enchantment>(e=>e.Enchantments);
            });

            modelBuilder.Entity<SaveBids>(entity =>
            {
                entity.HasIndex(e => e.BidderId);
            });

            modelBuilder.Entity<NbtData>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.UuId);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => e.UId);
                //entity.Property(e=>e.Id).ValueGeneratedOnAdd();
                //entity.HasMany(p=>p.Auctions).WithOne().HasForeignKey(a=>a.SellerId).HasPrincipalKey(p=>p.Id);
                //entity.HasMany(p=>p.Bids).WithOne().HasForeignKey(a=>a.BidderId).HasPrincipalKey(p=>p.Id);

            });


            modelBuilder.Entity<Coflnet.Sky.Items.Client.Model.Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Tag).IsUnique();
            });

            modelBuilder.Entity<AlternativeName>(entity =>
            {
                entity.HasIndex(e => e.Name);
            });


            modelBuilder.Entity<AveragePrice>(entity =>
            {
                entity.HasIndex(e => new { e.ItemId, e.Date }).IsUnique();
            });

            modelBuilder.Entity<Enchantment>(entity =>
            {
                entity.HasIndex(e => new { e.ItemType, e.Type, e.Level });
            });

            modelBuilder.Entity<GoogleUser>(entity =>
            {
                entity.HasIndex(e => e.GoogleId);
            });

            modelBuilder.Entity<NBTLookup>(entity =>
            {
                entity.HasKey(e => new { e.AuctionId, e.KeyId });
                entity.HasIndex(e => new { e.KeyId, e.Value });
            });

            modelBuilder.Entity<NBTKey>(entity =>
            {
                entity.HasIndex(e => e.Slug);
            });

            modelBuilder.Entity<Bonus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
            });
        }
    }
}

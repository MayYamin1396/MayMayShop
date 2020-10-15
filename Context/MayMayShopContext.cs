
using Microsoft.EntityFrameworkCore;
using MayMayShop.API.Models;
using System.Linq;

namespace MayMayShop.API.Context
{
    public partial class MayMayShopContext : DbContext
    {
        public MayMayShopContext()
        {
        }

        public MayMayShopContext(DbContextOptions<MayMayShopContext> options)
            : base(options)
        {
        }
        
        // public virtual DbSet<City> City { get; set; }

        // public virtual DbSet<DeliveryService> DeliveryService { get; set; }

        // public virtual DbSet<DeliveryServiceRate> DeliveryServiceRate { get; set; }        

        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }

        public virtual DbSet<MessageTemplate> MessageTemplate { get; set; }

        public virtual DbSet<Notification> Notification { get; set; }

        public virtual DbSet<NotificationTemplate> NotificationTemplate { get; set; }

        public virtual DbSet<Order> Order { get; set; }

        public virtual DbSet<OrderDeliveryInfo> OrderDeliveryInfo { get; set; }

        public virtual DbSet<OrderDetail> OrderDetail { get; set; }

        public virtual DbSet<OrderPaymentInfo> OrderPaymentInfo { get; set; }

        public virtual DbSet<OrderStatus> OrderStatus { get; set; }

        public virtual DbSet<PaymentService> PaymentService { get; set; }

        public virtual DbSet<PaymentStatus> PaymentStatus { get; set; }

        public virtual DbSet<Product> Product { get; set; }

        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        
        public virtual DbSet<ProductDiscount> ProductDiscount { get; set; }

        public virtual DbSet<ProductImage> ProductImage { get; set; }

        public virtual DbSet<ProductPrice> ProductPrice { get; set; }

        public virtual DbSet<ProductPromotion> ProductPromotion { get; set; }

        public virtual DbSet<ProductSerialOrder> ProductSerialOrder { get; set; }

        public virtual DbSet<ProductSku> ProductSku { get; set; }

        public virtual DbSet<ProductSkuValue> ProductSkuValue { get; set; }

        public virtual DbSet<ProductVariantOption> ProductVariantOption { get; set; }

        public virtual DbSet<Setting> Setting { get; set; }

        public virtual DbSet<Store> Store { get; set; }

        // public virtual DbSet<Township> Township { get; set; }

        public virtual DbSet<TrnCart> TrnCart { get; set; }

        public virtual DbSet<TrnCartDeliveryInfo> TrnCartDeliveryInfo { get; set; }
        
        public virtual DbSet<TrnProduct> TrnProduct { get; set; }

        public virtual DbSet<TrnProductSku> TrnProductSku { get; set; }

        public virtual DbSet<TrnProductSkuValue> TrnProductSkuValue { get; set; }

        public virtual DbSet<TrnProductVariantOption> TrnProductVariantOption { get; set; }

        public virtual DbSet<Variant> Variant { get; set; }
        public virtual DbSet<ProductTag> ProductTag { get; set; }
        public virtual DbSet<ProductClip> ProductClip { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<OrderTransaction> OrderTransaction {get;set;}
        public virtual DbSet<CategoryIcon> CategoryIcon {get;set;}
        public virtual DbSet<PaySlip> PaySlip {get;set;}
        public virtual DbSet<ProductReward> ProductReward {get;set;}
        public virtual DbSet<Policy> Policy {get;set;}
        public virtual DbSet<Banner> Banner {get;set;}
        public virtual DbSet<BannerLink> BannerLink {get;set;}
        public virtual DbSet<ActivityLog> ActivityLog {get;set;}
        public virtual DbSet<ActivityType> ActivityType {get;set;}
        public virtual DbSet<Platform> Platform {get;set;}
        public virtual DbSet<SearchKeyword> SearchKeyword {get;set;}
        public virtual DbSet<SearchKeywordTrns> SearchKeywordTrns {get;set;}
        public virtual DbSet<ProductRewardHistory> ProductRewardHistory {get;set;}
        public virtual DbSet<ProductType> ProductType {get;set;}

        //new table
        public virtual DbSet<Brand> Brand {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }                

            // modelBuilder.HasSequence<int>("TownId_seq", schema: "dbo")
            //     .StartsAt(0)
            //     .IncrementsBy(1);

            // modelBuilder.Entity<Township>()
            //     .Property(c => c.Id)
            //     .HasDefaultValueSql("NEXT VALUE FOR dbo.TownId_seq");

            // modelBuilder.HasSequence<int>("CityId_seq", schema: "dbo")
            //     .StartsAt(0)
            //     .IncrementsBy(1);

            // modelBuilder.Entity<City>()
            //     .Property(c => c.Id)
            //     .HasDefaultValueSql("NEXT VALUE FOR dbo.CityId_seq");

            modelBuilder.Entity<ProductSku>()
                .HasKey(k => new {k.ProductId, k.SkuId});

            modelBuilder.Entity<ProductSkuValue>()
                .HasKey(k => new {k.ProductId, k.SkuId, k.VariantId, k.ValueId});
            
            modelBuilder.Entity<ProductVariantOption>()
                .HasKey(k => new {k.ProductId, k.VariantId, k.ValueId});

            modelBuilder.Entity<ProductSerialOrder>()
                .HasKey(k => new {k.ProductId});

            modelBuilder.Entity<TrnProductSku>()
                .HasKey(k => new {k.ProductId, k.SkuId});

            modelBuilder.Entity<TrnProductSkuValue>()
                .HasKey(k => new {k.ProductId, k.SkuId, k.VariantId, k.ValueId});
            
            modelBuilder.Entity<TrnProductVariantOption>()
                .HasKey(k => new {k.ProductId, k.VariantId, k.ValueId});

            modelBuilder.Entity<TrnCart>()
                .HasKey(k => new {k.ProductId, k.SkuId, k.UserId});

            // modelBuilder.Entity<DeliveryServiceRate>()
            //     .HasKey(k => new {k.DeliveryServiceId, k.CityId, k.TownshipId});      
            
            modelBuilder.Entity<ProductTag>()
                .HasKey(k => new {k.ProductId, k.TagId});

            modelBuilder.Entity<ProductClip>()
                .HasKey(k => new {k.Name, k.ProductId});     
            
            base.OnModelCreating(modelBuilder);
        }

    }
}

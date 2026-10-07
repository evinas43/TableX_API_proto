using Microsoft.EntityFrameworkCore;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users", t =>
                t.HasCheckConstraint("CK_users_role", $"[role] IN ('{Roles.Camarero}', '{Roles.Caja}')"));

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.Username).HasColumnName("username").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasColumnType("varchar(255)").HasMaxLength(255).IsUnicode(false).IsRequired();
            e.Property(x => x.Role).HasColumnName("role").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();

            e.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Mesa>(e =>
        {
            e.ToTable("mesas");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.Numero).HasColumnName("numero").IsRequired();

            e.HasIndex(x => x.Numero).IsUnique();
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("productos", t =>
            {
                t.HasCheckConstraint("CK_productos_type", $"[type] IN ('{TiposProducto.Comestible}', '{TiposProducto.Bebida}')");
                t.HasCheckConstraint("CK_productos_precio", "[precio] >= 0");
            });

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(100)").HasMaxLength(100).IsUnicode(false).IsRequired();
            e.Property(x => x.Type).HasColumnName("type").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
            e.Property(x => x.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)").HasPrecision(10, 2).IsRequired();
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.ToTable("pedidos", t =>
                t.HasCheckConstraint("CK_pedidos_estado", $"[estado] IN ('{EstadosPedido.Abierto}', '{EstadosPedido.Cerrado}')"));

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.MesaId).HasColumnName("mesa_id").IsRequired();
            e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            e.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            e.Property(x => x.Estado).HasColumnName("estado").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false)
                .IsRequired().HasDefaultValue(EstadosPedido.Abierto);

            e.HasOne(x => x.Mesa).WithMany(m => m.Pedidos)
                .HasForeignKey(x => x.MesaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.User).WithMany(u => u.Pedidos)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetallePedido>(e =>
        {
            e.ToTable("detalle_pedido", t =>
            {
                t.HasCheckConstraint("CK_detalle_pedido_cantidad", "[cantidad] > 0");
                t.HasCheckConstraint("CK_detalle_pedido_precio", "[precio] >= 0");
            });

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.PedidoId).HasColumnName("pedido_id").IsRequired();
            e.Property(x => x.ProductoId).HasColumnName("producto_id").IsRequired();
            e.Property(x => x.Cantidad).HasColumnName("cantidad").IsRequired();
            e.Property(x => x.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)").HasPrecision(10, 2).IsRequired();
            e.Property(x => x.Pagado).HasColumnName("pagado").HasColumnType("bit").IsRequired().HasDefaultValue(false);

            e.HasOne(x => x.Pedido).WithMany(p => p.Detalles)
                .HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Producto).WithMany(p => p.Detalles)
                .HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pago>(e =>
        {
            e.ToTable("pagos", t =>
            {
                t.HasCheckConstraint("CK_pagos_importe", "[importe] > 0");
                t.HasCheckConstraint("CK_pagos_metodo", $"[metodo] IN ('{MetodosPago.Efectivo}', '{MetodosPago.Tarjeta}')");
            });

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            e.Property(x => x.PedidoId).HasColumnName("pedido_id").IsRequired();
            e.Property(x => x.Importe).HasColumnName("importe").HasColumnType("decimal(10,2)").HasPrecision(10, 2).IsRequired();
            e.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()").ValueGeneratedOnAdd();
            e.Property(x => x.Metodo).HasColumnName("metodo").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();

            e.HasOne(x => x.Pedido).WithMany(p => p.Pagos)
                .HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

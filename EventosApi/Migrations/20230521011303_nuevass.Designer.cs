﻿// <auto-generated />
using System;
using EventosApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventosApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230521011303_nuevass")]
    partial class nuevass
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EventoOrganizador", b =>
                {
                    b.Property<int>("EventosEventoId")
                        .HasColumnType("int");

                    b.Property<int>("OrganizadoresOrganizadorId")
                        .HasColumnType("int");

                    b.HasKey("EventosEventoId", "OrganizadoresOrganizadorId");

                    b.HasIndex("OrganizadoresOrganizadorId");

                    b.ToTable("EventoOrganizador");
                });

            modelBuilder.Entity("EventoUsuario", b =>
                {
                    b.Property<int>("FavoritosEventoId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioFavoritosUsuarioId")
                        .HasColumnType("int");

                    b.HasKey("FavoritosEventoId", "UsuarioFavoritosUsuarioId");

                    b.HasIndex("UsuarioFavoritosUsuarioId");

                    b.ToTable("EventoUsuario");
                });

            modelBuilder.Entity("EventosApi.Data.Evento", b =>
                {
                    b.Property<int>("EventoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventoId"));

                    b.Property<int>("Capacidad_Maxima")
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre_Evento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ubicacion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventoId");

                    b.ToTable("Eventos");
                });

            modelBuilder.Entity("EventosApi.Data.Organizador", b =>
                {
                    b.Property<int>("OrganizadorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrganizadorId"));

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrganizadorId");

                    b.ToTable("Organizadores");
                });

            modelBuilder.Entity("EventosApi.Data.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioId"));

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UsuarioId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("OrganizadorUsuario", b =>
                {
                    b.Property<int>("SeguidoresUsuarioId")
                        .HasColumnType("int");

                    b.Property<int>("seguidosOrganizadorId")
                        .HasColumnType("int");

                    b.HasKey("SeguidoresUsuarioId", "seguidosOrganizadorId");

                    b.HasIndex("seguidosOrganizadorId");

                    b.ToTable("OrganizadorUsuario");
                });

            modelBuilder.Entity("EventoOrganizador", b =>
                {
                    b.HasOne("EventosApi.Data.Evento", null)
                        .WithMany()
                        .HasForeignKey("EventosEventoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventosApi.Data.Organizador", null)
                        .WithMany()
                        .HasForeignKey("OrganizadoresOrganizadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EventoUsuario", b =>
                {
                    b.HasOne("EventosApi.Data.Evento", null)
                        .WithMany()
                        .HasForeignKey("FavoritosEventoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventosApi.Data.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuarioFavoritosUsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OrganizadorUsuario", b =>
                {
                    b.HasOne("EventosApi.Data.Usuario", null)
                        .WithMany()
                        .HasForeignKey("SeguidoresUsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventosApi.Data.Organizador", null)
                        .WithMany()
                        .HasForeignKey("seguidosOrganizadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

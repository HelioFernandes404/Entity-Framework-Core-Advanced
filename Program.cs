﻿using System;
using System.Text.Json;
using DominandoEFCore.Domain;
using EFcore.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            ExemploTPH();
        }

        static void ExemploTPH()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var pessoa = new Pessoa { Nome = "Fulano de tal" };
            var instrutor = new Instrutor { Nome = "Beltrano", Tecnologia = ".NET", Desde = DateTime.Now };
            var aluno = new Aluno { Nome = "Sicrano", Idade = 25, DataContrato = DateTime.Now.AddDays(-1) };

            db.AddRange(pessoa, instrutor, aluno);
            db.SaveChanges();

            var pessoas = db.Set<Pessoa>().AsNoTracking().ToList();
            var instrutores = db.Set<Instrutor>().AsNoTracking().ToList();
            var alunos = db.Set<Aluno>().AsNoTracking().ToList();

            Console.WriteLine("Pessoas");
            pessoas.ForEach(p => Console.WriteLine($"Id: {p.Id}, Nome: {p.Nome}"));

            Console.WriteLine("Instrutores");
            instrutores.ForEach(i => Console.WriteLine($"Id: {i.Id}, Nome: {i.Nome}, Tecnologia: {i.Tecnologia}"));

            Console.WriteLine("Alunos");
            alunos.ForEach(a => Console.WriteLine($"Id: {a.Id}, Nome: {a.Nome}, Idade: {a.Idade}"));
        }
        static void RelaciomentoUmParaMuitos()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "Sergipe",
                Governador = new Governador { Nome = "Beltrano" }
            };

            estado.Cidades.Add(new Cidade { Nome = "Itabaiana" });

            db.Estados.Add(estado);
            db.SaveChanges();

            using (var dbContext = new ApplicationContext())
            {
                var estados = dbContext.Estados.ToList();

                estados[0].Cidades.Add(new Cidade { Nome = "Aracaju" });

                dbContext.SaveChanges();

                foreach (var est in db.Estados.AsNoTracking().Include(e => e.Cidades))
                {
                    Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");

                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\tCidade: {cidade.Nome}");
                    }
                }
            }
        }
        static void RelaciomentoUmParaUm()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "Sergipe",
                Governador = new Governador { Nome = "Beltrano" }
            };

            db.Estados.Add(estado);

            db.SaveChanges();

            var estados = db.Estados.AsNoTracking().ToList();

            estados.ForEach(e =>
            {
                Console.WriteLine($"Estado: {e.Nome} - Governador: {e.Governador.Nome}");
            });
        }
        static void TiposDePropriedades()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();


            var cliente = new Cliente
            {
                Nome = "Fulano de tal",
                Telefone = "(79) 99999-9999",
                Endereco = new Endereco { Logradouro = "Rua das flores", Bairro = "Bairro das flores", Cidade = "Cidade das flores", Estado = "SE" }
            };

            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList(); // AsNoTracking() não rastreia as entidades

            var ops = new JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, ops);
                Console.WriteLine(json);
            });
        }
        static void TrabalhandoComPropriedadeDeSombra()
        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento { Descricao = "Departamento Propriedade de Sombra" };

            db.Departamentos.Add(departamento);

            db.Entry<Departamento>(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            db.SaveChanges();

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") > DateTime.Now.AddDays(-1)).ToList();
        }
        static void ShadowProperties()

        {
            using var db = new ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento { Descricao = "Departamento Teste" };
        }

    }
}
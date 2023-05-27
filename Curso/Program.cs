using System;
using CursoEFCore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using System.Collections.Generic;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // Este snippet de código executa as migrações no banco de dados
            // Atenção, não deve ser usado em Produção, apenas para efeitos de desenvolvimento
            //using var dbContext = new ApplicationDbContext();
            //dbContext.Database.Migrate();

            //Verificar se existem Migrações pendentes
            // using var dbContext = new ApplicationDbContext();
            // var pendingMigrations = dbContext.Database.GetPendingMigrations().Any();

            // if(pendingMigrations) Console.WriteLine("There are pending migrations to be executed");
            //InserirDados();
            //InserirDadosEmMassa();
            //ConsultarDados();
            //CadastrarPedido();
            //ConsultarPedidoEagerLoading();
            //ActualizarDados();
            RemoverRegistro();
        }
        
        private static void RemoverRegistro()
        {
            using var db = new ApplicationDbContext();
            //var cliente = db.Clientes.Find(5);
            // db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            // db.Entry(cliente).State = EntityState.Deleted;
            //cenário desconectado
            var cliente = new Cliente{ Id = 5};
            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges(); 
        }
        private static void ActualizarDados()
        {
            using var db = new ApplicationDbContext();
            //var cliente = db.Clientes.Find(3); //ou db.Clientes.FirstOrDefault(p => p.Id == 1)
            //assim apenas actualzia o campo "Nome"
            //cliente.Nome = "Cliente alterado passo 2";
            
           
            //as duas instruções seguintes actualizam todas os campo da entidade cliente, no banco de dados
            //db.Clientes.Update(cliente);
            //db.Entry(cliente).State = EntityState.Modified;

            //as duas instruções seguintes actualizam os campos "Nome" e "Telefone" da entidade Cliente, de forma desconectada
            //  var clienteDesconectado = new {
            //     Nome = "Cliente alterado 3",
            //     Telefone = "263290742"
            // };
            // db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            //as três instruções seguintes actualizam todos os camposs "Nome" e "Telefone" da entidade Cliente, usando entidade Cliente de forma desconectada
            var cliente = new Cliente
            {
                Id = 1
            };
            var clienteDesconectado = new {
                 Nome = "Cliente alterado 4",
                 Telefone = "123456789"
            };
            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            db.SaveChanges();
        }

        private static void ConsultarPedidoEagerLoading()
        {
            using var db = new ApplicationDbContext();

            var pedidos = db.Pedidos.Include(p => p.Itens)
                                    .ThenInclude(p => p.Produto)
                                    .Include(p => p.Cliente)
                                    .ToList();
            Console.WriteLine(pedidos.Count);
        }

        private static void CadastrarPedido()
        {
            using var db = new ApplicationDbContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void InserirDadosEmMassa()
        {
            // var produto = new Produto
            // {
            //     Descricao = "Produto Teste",
            //     CodigoBarras = "123456789012",
            //     Valor = 10m,
            //     TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
            //     Ativo = true
            // };

            // var cliente = new Cliente
            // {
            //     Nome = "Nelson Francisco Vicente Soares",
            //     CEP = "90420140",
            //     Cidade = "Porto Alegre",
            //     Estado = "RS",
            //     Telefone = "9976681787"
            // };
            var clientes = new []{
                new Cliente
                {
                    Nome = "Nelson Francisco Vicente Soares",
                    CEP = "90420140",
                    Cidade = "Porto Alegre",
                    Estado = "RS",
                    Telefone = "9976681787"
                },
                new Cliente
                {
                    Nome = "Teste",
                    CEP = "12345100",
                    Cidade = "Rio Janeiro",
                    Estado = "RJ",
                    Telefone = "9976681787"
                }   
            };
            using var db = new ApplicationDbContext();
            // db.AddRange(produto, cliente);
            db.Clientes.AddRange(clientes);
            var registros = db.SaveChanges();
            Console.WriteLine($"Total registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new ApplicationDbContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                                        .Where(p => p.Id > 0)
                                        .OrderBy(p => p.Id)
                                        .ToList();
            foreach(var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando cliente {cliente.Id}");
                //db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            }
        }
        
        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "123456789012",
                Valor = 10m,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationDbContext();
            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total registro(s): {registros}");
        }
    }
}

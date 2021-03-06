﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EFCore.Domains;
using EFCore.Interfaces;
using EFCore.Repositories;
using EFCore.Utills;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Senai.EfCore.Tarde.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        // readonly significa somente leitura, sem alterações
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController()
        {
            _produtoRepository = new ProdutoRepository();
        }

        [HttpGet]
        // IActionResult vou retornar resultado da minha ação
        public IActionResult Get()
        {
            // tentar
            try
            {
                // Vamos listar os produtos do repository
                var produtos = _produtoRepository.LerTodos();

                // criar uma condição para verificar a existencia desses produtos, caso n exista retorna
                //No content - Sem conteúdo
                if (produtos.Count == 0)
                    return NoContent();

                //Caso exista retorna ok e os produtos existentes
                return Ok(new
                {
                    // retornamos mais informações para o nosso frontend como a quantidade de produtos e seus dados
                    TotalCount = produtos.Count,
                    data = produtos
                });
            }
            // caso
            catch (Exception)
            {
                // to do: Cadastrar mensagem de erro no dominio log erro
                return BadRequest(new
                {
                    statusCode = 400,
                    error = "Ocorreu um erro no endpoint Get/produtos. Envie um e-mail para email@gmail.com informando"
                });
            }
        }

        // GET api/<RacaController>/5
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            try
            {
                // Busco o produto pelo id lá no repositorio
                var produto = _produtoRepository.BuscarPorId(id);

                // Aqui nós fazemos uma verificação para saber se esse produto buscado existe. Caso n exista retorna
                // Retorna Notfound- Produto n encontrado
                if (produto == null)

                    return NotFound();

                // Caso o produto exista retorna
                // Ok e seus dados
                return Ok(id);

            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro retorna BadRequest e a msg de erro
                return BadRequest(ex.Message);
            }
        }

        // POST api/<AlunoController>
        [HttpPost]
        // passou como parametro um formulario
        public IActionResult Post([FromForm] Produto produto)
        {
            try
            {
                // verifica se foi enviado um arquivo com a imagem
                if (produto.Imagem != null)
                {
                    var urlImagem = Upload.Local(produto.Imagem);

                    produto.UrlImagem = urlImagem;
                }

                // Adiciona um produto
                _produtoRepository.Cadastrar(produto);

                // retorna ok com os dados do produto cadastrado
                return Ok(produto);
            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro retorna BadRequest e a msg de erro
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<AlunoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, Produto produto)
        {
            try
            {
                //certamente se ele passou pelo buscar Id ele existe
                var produtoTemp = _produtoRepository.BuscarPorId(id);

                // fAZ A verificação do produto
                if (produtoTemp == null)
                    return NotFound();

                // retorna ok com os dados do produto alterado
                return Ok(produto);
            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro retorna BadRequest e a msg de erro
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<AlunoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Remove o produto pelo id
                _produtoRepository.Excluir(id);

                // em caso de sucesso de sua remoção
                //retorna um ok e mostra o Id 
                return Ok(id);


            }
            catch (Exception ex)
            {

                // Caso ocorra algum erro retorna BadRequest e a msg de erro
                return BadRequest(ex.Message);
            }
        }
    }
}
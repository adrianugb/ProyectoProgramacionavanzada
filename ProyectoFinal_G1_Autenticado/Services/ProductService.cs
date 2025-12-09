using System;
using System.Collections.Generic;
using ProyectoFinal_G1_Autenticado.Models;
using ProyectoFinal_G1_Autenticado.Repositories;

namespace ProyectoFinal_G1_Autenticado.Services
{
    public class ProductService : IProductService, IDisposable
    {
        private IProductRepository productRepo;
        private IProductImageRepository imageRepo;

        public ProductService()
        {
            productRepo = new ProductRepository();
            imageRepo = new ProductImageRepository();
        }

        public IEnumerable<Product> GetAll()
        {
            return productRepo.GetAll();
        }

        public IEnumerable<Product> GetActiveProducts()
        {
            return productRepo.GetActiveProducts();
        }

        public Product GetById(int id)
        {
            return productRepo.GetById(id);
        }

        public Product GetWithImages(int id)
        {
            return productRepo.GetWithImages(id);
        }

        public Product GetByCode(string code)
        {
            return productRepo.GetByCode(code);
        }

        public void Create(Product product)
        {
            productRepo.Add(product);
            productRepo.Save();
        }

        public void Update(Product product)
        {
            productRepo.Update(product);
            productRepo.Save();
        }

        public void Delete(int id)
        {
            var product = productRepo.GetWithImages(id);
            if (product != null)
            {
                // Eliminar imagenes primero
                var images = imageRepo.GetByProductId(id);
                foreach (var img in images)
                {
                    imageRepo.Delete(img);
                }
                imageRepo.Save();

                productRepo.Delete(product);
                productRepo.Save();
            }
        }

        public void ToggleStatus(int id)
        {
            var product = productRepo.GetById(id);
            if (product != null)
            {
                product.Status = product.Status == ProductStatus.Activo
                    ? ProductStatus.Inactivo
                    : ProductStatus.Activo;
                productRepo.Update(product);
                productRepo.Save();
            }
        }

        public void Dispose()
        {
            (productRepo as IDisposable)?.Dispose();
            (imageRepo as IDisposable)?.Dispose();
        }
    }
}


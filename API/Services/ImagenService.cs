using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class ImagenService
    {
        private readonly IImagenRepository _repo;
        private readonly IProductoRepository _productoRepo;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _extensionesPermitidas = [".jpg", ".jpeg", ".png"];
        private static readonly string[] _tiposMimePermitidos = ["image/jpeg", "image/png"];
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ImagenService(IImagenRepository repo, IProductoRepository productoRepo, IWebHostEnvironment env)
        {
            _repo = repo;
            _productoRepo = productoRepo;
            _env = env;
        }

        public async Task<Imagen> SubirImagen(int productoId, IFormFile archivo)
        {
            if (!await _productoRepo.Existe(productoId))
            {
                throw new RecursoNoExisteException("No existe ningún producto con ese id.");
            }

            if (archivo is null || archivo.Length == 0)
            {
                throw new DatosLlegaronErradosException("Debe proporcionar un archivo de imagen válido.");
            }

            if (archivo.Length > MaxFileSize)
            {
                throw new DatosLlegaronErradosException("El archivo excede el tamaño máximo permitido de 5MB.");
            }

            string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!_extensionesPermitidas.Contains(extension))
            {
                throw new DatosLlegaronErradosException("Formato no permitido. Solo se aceptan archivos PNG y JPG/JPEG.");
            }

            if (!_tiposMimePermitidos.Contains(archivo.ContentType.ToLowerInvariant()))
            {
                throw new DatosLlegaronErradosException("Tipo MIME inválido para la imagen.");
            }

            try
            {
                // Como es 1 a 1: si ya había una imagen para este producto, la reemplazamos.
                Imagen? imagenExistente = await _repo.ObtenerPorProductoId(productoId);
                if (imagenExistente is not null)
                {
                    EliminarArchivoFisico(imagenExistente.RutaRelativa);
                    await _repo.Eliminar(imagenExistente);
                }

                string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadsDir = Path.Combine(webRoot, "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                string nombreUnico = $"{Guid.NewGuid()}{extension}";
                string rutaFisicaCompleta = Path.Combine(uploadsDir, nombreUnico);

                await using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                Imagen imagen = new Imagen
                {
                    NombreOriginal = Path.GetFileName(archivo.FileName),
                    NombreArchivo = nombreUnico,
                    RutaRelativa = $"uploads/{nombreUnico}",
                    TipoContenido = archivo.ContentType,
                    TamanoBytes = archivo.Length,
                    ProductoId = productoId
                };

                await _repo.Crear(imagen);

                return imagen;
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        private void EliminarArchivoFisico(string rutaRelativa)
        {
            string webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string rutaFisica = Path.Combine(webRoot, rutaRelativa);
            if (File.Exists(rutaFisica))
            {
                File.Delete(rutaFisica);
            }
        }
    }
}

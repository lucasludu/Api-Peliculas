﻿using ApiPelicula.Models;
using System.Collections.Generic;

namespace ApiPelicula.Repository.IRepository
{
    public interface IPeliculaRepository
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasEnCategoria(int CatId);
        Pelicula GetPelicula(int PeliculaId);
        bool ExistePelicula(string nombre);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        bool ExistePelicula(int id);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool BorrarPelicula(Pelicula pelicula);
        bool Guardar();
    }
}

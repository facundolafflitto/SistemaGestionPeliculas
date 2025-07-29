import React from "react";

const PeliculaCard = ({ pelicula, onEdit, onDelete, esAdmin }) => (
  <div className="bg-white dark:bg-gray-800 rounded shadow hover:shadow-lg transition p-4 border border-gray-200 dark:border-gray-700">
    <img
      src={pelicula.imagenUrl}
      alt={pelicula.titulo}
      className="w-full h-64 object-contain rounded-t mb-3 bg-white dark:bg-gray-900"
    />
    <h3 className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-1">
      {pelicula.titulo}
    </h3>
    <p className="text-sm text-gray-600 dark:text-gray-300">
      {pelicula.genero}
    </p>
    <p className="text-sm text-gray-500 dark:text-gray-400 mb-2">
      {pelicula.año}
    </p>
    {esAdmin && (
      <div className="flex justify-between">
        <button
          onClick={() => onEdit(pelicula)}
          className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
        >
          Editar
        </button>
        <button
          onClick={() => onDelete(pelicula.id)}
          className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
        >
          Eliminar
        </button>
      </div>
    )}
  </div>
);

export default PeliculaCard;

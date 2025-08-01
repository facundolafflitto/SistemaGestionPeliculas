import React from "react";

const PeliculaCard = ({
  pelicula,
  onEdit,
  onDelete,
  esAdmin,
  esFavorita,
  onToggleFavorita,
  onVerDetalle,
}) => (
  <div className="bg-white dark:bg-gray-800 rounded shadow hover:shadow-lg transition p-4 border border-gray-200 dark:border-gray-700 relative">
    <img
      src={pelicula.imagenUrl}
      alt={pelicula.titulo}
      className="w-full h-64 object-contain rounded-t mb-3 bg-white dark:bg-gray-900 cursor-pointer"
      onClick={onVerDetalle}
    />
    <h3
      className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-1 cursor-pointer"
      onClick={onVerDetalle}
      title="Ver detalle"
      style={{ userSelect: "none" }}
    >
      {pelicula.titulo}
    </h3>
    <p className="text-sm text-gray-600 dark:text-gray-300">{pelicula.genero}</p>
    <p className="text-sm text-gray-500 dark:text-gray-400 mb-2">{pelicula.año}</p>
    {/* Botón de favorito */}
{onToggleFavorita && (
  <button
    className="absolute top-2 right-2 text-2xl z-50 cursor-pointer"
    onClick={e => {
      e.stopPropagation();
      e.preventDefault();
      onToggleFavorita();
    }}
    title={esFavorita ? "Quitar de favoritas" : "Agregar a favoritas"}
    aria-label="Favorita"
    style={{ background: "none", border: "none" }}
  >
    {esFavorita ? "❤️" : "🤍"}
  </button>
)}
    {esAdmin && (
      <div className="flex justify-between mt-2">
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

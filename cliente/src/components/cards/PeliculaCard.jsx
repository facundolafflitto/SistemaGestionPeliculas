import React from "react";

export default function PeliculaCard({
  pelicula,
  esFavorita,
  onToggleFavorita,
  onVerDetalle,
}) {
  return (
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

      <p className="text-gray-600 dark:text-gray-300 text-sm mb-1">
        {pelicula.genero}
      </p>
      <p className="text-gray-500 dark:text-gray-400 text-sm">
        {pelicula.año}
      </p>

      {onToggleFavorita && (
        <button
          type="button"
          className="absolute top-2 right-2 z-50 cursor-pointer pointer-events-auto select-none rounded-full px-2 py-1 text-2xl bg-transparent border-0"
          onClick={(e) => {
            e.stopPropagation();
            e.preventDefault();
            onToggleFavorita();
          }}
          title={esFavorita ? "Quitar de favoritas" : "Agregar a favoritas"}
          aria-label="Favorita"
          aria-pressed={!!esFavorita}
        >
          {esFavorita ? "❤️" : "🤍"}
        </button>
      )}
    </div>
  );
}

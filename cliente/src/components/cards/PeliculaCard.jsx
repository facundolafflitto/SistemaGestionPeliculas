import React from "react";

export default function PeliculaCard({
  pelicula,
  esFavorita,
  onToggleFavorita,
  onVerDetalle,
  esAdmin,        // 👈 nuevo
  onEdit,         // 👈 nuevo
  onDelete,       // 👈 nuevo
}) {
  return (
    <div className="relative bg-white dark:bg-gray-800 rounded shadow hover:shadow-lg transition p-4 border border-gray-200 dark:border-gray-700">
      {/* Imagen (clic para ver detalle) */}
      <div
        role="button"
        tabIndex={0}
        className="w-full h-64 rounded-t mb-3 bg-white dark:bg-gray-900 cursor-pointer"
        onClick={onVerDetalle}
        onKeyDown={(e) => (e.key === "Enter" ? onVerDetalle?.() : null)}
      >
        <img
          src={pelicula.imagenUrl}
          alt={pelicula.titulo}
          className="w-full h-full object-contain"
          loading="lazy"
        />
      </div>

      {/* Título (clic para ver detalle) */}
      <h3
        className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-1 cursor-pointer"
        title="Ver detalle"
        onClick={onVerDetalle}
        style={{ userSelect: "none" }}
      >
        {pelicula.titulo}
      </h3>

      <p className="text-gray-600 dark:text-gray-300 text-sm mb-1">{pelicula.genero}</p>
      <p className="text-gray-500 dark:text-gray-400 text-sm">{pelicula.año}</p>

      {/* Corazón (favorita) */}
      {onToggleFavorita && (
        <button
          type="button"
          className="absolute top-2 right-2 z-50 cursor-pointer select-none rounded-full px-2 py-1 text-2xl bg-transparent border-0"
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

      {/* Acciones de admin */}
      {esAdmin && (
        <div className="mt-3 flex gap-2">
          <button
            type="button"
            className="px-3 py-1 text-sm rounded bg-blue-600 hover:bg-blue-700 text-white"
            onClick={(e) => {
              e.stopPropagation();
              onEdit?.(pelicula);
            }}
            title="Editar"
          >
            Editar
          </button>
          <button
            type="button"
            className="px-3 py-1 text-sm rounded bg-red-600 hover:bg-red-700 text-white"
            onClick={(e) => {
              e.stopPropagation();
              onDelete?.(pelicula.id);
            }}
            title="Eliminar"
          >
            Eliminar
          </button>
        </div>
      )}
    </div>
  );
}

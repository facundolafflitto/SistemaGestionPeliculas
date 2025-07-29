import React, { useState } from "react";

const API_URL = import.meta.env.VITE_API_URL;

const FormularioPelicula = ({
  pelicula,
  setPelicula,
  guardarCambios,
  cancelarEdicion
}) => {
  const [errores, setErrores] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setPelicula((prev) => ({ ...prev, [name]: value }));
  };

  const validarFormulario = () => {
    const erroresDetectados = [];

    if (!pelicula.titulo?.trim()) erroresDetectados.push("El título es obligatorio.");
    if (!pelicula.genero?.trim()) erroresDetectados.push("El género es obligatorio.");
    if (!pelicula.año?.toString().trim()) {
      erroresDetectados.push("El año es obligatorio.");
    } else if (isNaN(parseInt(pelicula.año))) {
      erroresDetectados.push("El año debe ser un número válido.");
    }

    setErrores(erroresDetectados.join("\n"));
    return erroresDetectados.length === 0;
  };

  const handleGuardar = () => {
    if (!validarFormulario()) return;
    guardarCambios();
  };

  const buscarInfoOMDb = async () => {
    if (!pelicula.titulo || pelicula.titulo.trim() === "") {
      setErrores("Primero escribí un título para buscar.");
      return;
    }
    try {
      const resp = await fetch(
        `${API_URL}/api/peliculas/buscar-omdb?titulo=${encodeURIComponent(
          pelicula.titulo
        )}`
      );
      if (!resp.ok) {
        setErrores("Película no encontrada en OMDb.");
        return;
      }
      const data = await resp.json();
      setPelicula((prev) => ({
        ...prev,
        titulo: data.titulo || prev.titulo,
        genero: data.genero || "",
        año: data.año || "",
        imagenUrl: data.imagenUrl !== "N/A" ? data.imagenUrl : ""
      }));
    } catch (error) {
      setErrores("Error consultando OMDb.");
      console.error(error);
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white dark:bg-gray-800 rounded shadow p-6 mb-6 border border-gray-200 dark:border-gray-700">
      <h3 className="text-2xl font-bold mb-4 text-center text-gray-700 dark:text-gray-100">
        {pelicula?.id ? "✏️ Editar Película" : "➕ Nueva Película"}
      </h3>

      {errores && (
        <div className="mb-4 text-red-500 text-sm whitespace-pre-line">
          {errores}
        </div>
      )}

      <div className="flex gap-2 mb-3">
        <input
          type="text"
          name="titulo"
          placeholder="Título"
          value={pelicula.titulo}
          onChange={handleChange}
          className="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-800 dark:text-white dark:placeholder-gray-400"
        />
        <button
          type="button"
          onClick={buscarInfoOMDb}
          className="bg-blue-600 text-white px-3 py-1 rounded hover:bg-blue-700 transition"
          title="Buscar info en OMDb"
        >
          Buscar info
        </button>
      </div>

      <input
        type="text"
        name="genero"
        placeholder="Género"
        value={pelicula.genero}
        onChange={handleChange}
        className="w-full p-2 mb-3 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-800 dark:text-white dark:placeholder-gray-400"
      />

      <input
        type="number"
        name="año"
        placeholder="Año"
        value={pelicula.año}
        onChange={handleChange}
        className="w-full p-2 mb-3 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-800 dark:text-white dark:placeholder-gray-400"
      />

      <input
        type="text"
        name="imagenUrl"
        placeholder="URL de la imagen"
        value={pelicula.imagenUrl}
        onChange={handleChange}
        className="w-full p-2 mb-4 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-800 dark:text-white dark:placeholder-gray-400"
      />

      {pelicula.imagenUrl && pelicula.imagenUrl !== "N/A" && (
        <img
          src={pelicula.imagenUrl}
          alt="Poster"
          className="mb-3 max-h-40 rounded shadow"
        />
      )}

      <div className="flex justify-between">
        <button
          type="button"
          onClick={cancelarEdicion}
          className="bg-gray-300 dark:bg-gray-600 text-gray-800 dark:text-white px-4 py-2 rounded hover:bg-gray-400 dark:hover:bg-gray-500 transition"
        >
          Cancelar
        </button>
        <button
          type="button"
          onClick={handleGuardar}
          className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 transition"
        >
          Guardar {pelicula?.id ? "cambios" : "película"}
        </button>
      </div>
    </div>
  );
};

export default FormularioPelicula;

import React, { useEffect, useRef, useState } from "react";

const DetalleModalSerie = ({ serie, onClose }) => {
  const fondoRef = useRef();
  const [detalle, setDetalle] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [temporadaIdx, setTemporadaIdx] = useState(null);
  const [error, setError] = useState(null);
  const [mostrarTodos, setMostrarTodos] = useState(false);

  const cacheRef = useRef({});
  const LIMITE_TEMPORADAS = 8;

  useEffect(() => {
    const handleKey = (e) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [onClose]);

  const handleClickFondo = (e) => {
    if (e.target === fondoRef.current) onClose();
  };

  useEffect(() => {
    if (!serie) return;

    const titulo = serie.titulo;
    if (cacheRef.current[titulo]) {
      setDetalle(cacheRef.current[titulo]);
      setCargando(false);
      return;
    }

    setCargando(true);
    setDetalle(null);
    setError(null);
    setTemporadaIdx(null);

    fetch(`http://localhost:5183/api/series/tmdb-detalle?titulo=${encodeURIComponent(titulo)}`)
      .then(async (res) => {
        if (!res.ok) {
          const err = await res.text();
          throw new Error("Error de backend: " + err);
        }
        return res.json();
      })
      .then((data) => {
        cacheRef.current[titulo] = data;
        setDetalle(data);
      })
      .catch((e) => {
        setDetalle(null);
        setError(e.message);
      })
      .finally(() => setCargando(false));
  }, [serie]);

  if (!serie) return null;

  return (
    <div
      ref={fondoRef}
      className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-70 z-50"
      onClick={handleClickFondo}
    >
      <div
        className="bg-white dark:bg-gray-900 rounded shadow-lg max-w-4xl w-full relative p-6 sm:p-8"
        style={{ maxHeight: "90vh", overflowY: "auto" }}
      >
        <button
          onClick={onClose}
          className="absolute top-3 right-3 text-2xl text-white bg-gray-700 hover:bg-red-600 rounded-full w-8 h-8 flex items-center justify-center shadow-lg"
          title="Cerrar"
        >
          ×
        </button>

        {error && (
          <div className="text-red-500 py-10 text-center">{error}</div>
        )}

        {!detalle || cargando ? (
          <div className="py-16 text-center text-lg text-gray-400">
            Cargando datos de la serie...
          </div>
        ) : (
          <>
            {/* Encabezado */}
            <div className="flex flex-col sm:flex-row gap-6 mb-6 items-start">
              {detalle.poster && (
                <img
                  src={detalle.poster}
                  alt={detalle.titulo}
                  className="rounded w-48 h-auto max-h-72 shadow-lg object-cover flex-shrink-0"
                />
              )}
              <div className="flex flex-col justify-between">
                <h2 className="text-3xl font-bold text-white mb-2">
                  {detalle.titulo}
                </h2>
                <p className="text-gray-300 text-sm bg-gray-800 p-3 rounded">
                  {detalle.overview || "Sin descripción."}
                </p>

                {/* Temporadas */}
                <div className="mt-4">
                  <div className="mb-2 font-semibold text-white">
                    Temporadas:
                  </div>
                  <div className="flex flex-wrap gap-2 py-2">
                    {(mostrarTodos
                      ? detalle.temporadas
                      : detalle.temporadas.slice(0, LIMITE_TEMPORADAS)
                    ).map((temp, idx) => (
                      <button
                        key={temp.numero}
                        className={`px-3 py-1 text-sm rounded font-medium transition ${
                          temporadaIdx === idx
                            ? "bg-purple-600 text-white"
                            : "bg-gray-700 text-gray-300 hover:bg-purple-800"
                        }`}
                        onClick={() =>
                          setTemporadaIdx(idx === temporadaIdx ? null : idx)
                        }
                      >
                        {temp.nombre || `Temp. ${temp.numero}`}
                      </button>
                    ))}

                    {detalle.temporadas.length > LIMITE_TEMPORADAS && (
                      <button
                        onClick={() => setMostrarTodos(!mostrarTodos)}
                        className="text-sm underline text-purple-400"
                      >
                        {mostrarTodos ? "Ver menos" : "Ver más"}
                      </button>
                    )}
                  </div>
                </div>
              </div>
            </div>

            {/* Episodios */}
            {temporadaIdx !== null &&
              detalle.temporadas &&
              detalle.temporadas[temporadaIdx] && (
                <div className="mt-4">
                  <h3 className="text-xl font-bold text-white mb-2">
                    {detalle.temporadas[temporadaIdx].nombre ||
                      `Temporada ${detalle.temporadas[temporadaIdx].numero}`}
                  </h3>
                  <p className="mb-3 text-gray-300 text-sm">
                    {detalle.temporadas[temporadaIdx].overview ||
                      "Sin descripción."}
                  </p>

                  {detalle.temporadas[temporadaIdx].episodios?.length === 0 ? (
                    <div className="py-10 text-center text-gray-400">
                      No hay episodios para esta temporada.
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 max-h-[350px] overflow-y-auto pr-1">
                      {detalle.temporadas[temporadaIdx].episodios?.map(
                        (ep, i) => (
                          <div
                            key={ep.numero || ep.nombre || i}
                            className="bg-gray-800 rounded-lg shadow hover:shadow-lg transition p-4 flex flex-col gap-2"
                          >
                            {ep.imagen ? (
                              <img
                                src={ep.imagen}
                                alt={ep.nombre || `Episodio ${ep.numero}`}
                                className="w-full h-32 object-cover rounded"
                              />
                            ) : (
                              <div className="w-full h-32 bg-gray-700 rounded flex items-center justify-center text-gray-400 text-xs text-center">
                                Sin imagen
                              </div>
                            )}
                            <div className="text-white font-bold">
                              {`Episodio ${ep.numero ?? "-"}`}
                            </div>
                            {ep.nombre && (
                              <div className="text-purple-400 font-medium">
                                {ep.nombre}
                              </div>
                            )}
                            <div className="text-sm text-gray-300 line-clamp-3">
                              {ep.overview?.trim() || "Sin descripción."}
                            </div>
                            <div className="text-xs text-gray-400 mt-auto">
                              {ep.fecha
                                ? `📅 Emitido: ${ep.fecha}`
                                : "📅 Fecha no disponible"}
                            </div>
                          </div>
                        )
                      )}
                    </div>
                  )}
                </div>
              )}
          </>
        )}
      </div>
    </div>
  );
};

export default DetalleModalSerie;

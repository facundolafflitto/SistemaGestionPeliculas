import { useEffect, useState } from "react";
const API_URL = import.meta.env.VITE_API_URL;

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const token = localStorage.getItem("token");

  useEffect(() => {
    fetch(`${API_URL}/api/me/dashboard`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(r => (r.ok ? r.json() : Promise.reject(r.statusText)))
      .then(setData)
      .catch(err => console.error("Dashboard error:", err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="p-6 text-gray-700 dark:text-gray-300">Cargando…</div>;
  if (!data) return <div className="p-6 text-gray-700 dark:text-gray-300">No se pudo cargar.</div>;

  return (
    <main className="w-full min-h-screen bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors">
      <div className="w-full px-4 sm:px-6 lg:px-10 py-6">
        <h1 className="text-2xl md:text-3xl font-bold flex items-center gap-2 mb-6">
          <span className="text-purple-500 text-3xl">👤</span> Mi Dashboard
        </h1>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <Kpi title="Películas favoritas" value={data.totalPeliculas} />
          <Kpi title="Series favoritas" value={data.totalSeries} />
          <Kpi title="Usuario" value={data.email} isText />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
          <Genres title="Top géneros en películas" items={data.topGenerosPeliculas} />
          <Genres title="Top géneros en series" items={data.topGenerosSeries} />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Favorites title="Tus películas favoritas" items={data.peliculasFavoritas} />
          <Favorites title="Tus series favoritas" items={data.seriesFavoritas} />
        </div>
      </div>
    </main>
  );
}

/* ---------- subcomponentes ---------- */

function Card({ children, className = "" }) {
  return (
    <div
      className={`rounded-xl p-4 shadow-sm border transition-colors
                     bg-white border-gray-200
                     dark:bg-gray-800/90 dark:border-white/10 ${className}`}
    >
      {children}
    </div>
  );
}

function Kpi({ title, value, isText = false }) {
  return (
    <Card>
      <div className="text-sm text-gray-500 dark:text-gray-400 mb-1">{title}</div>
      <div className={`font-semibold ${isText ? "text-base truncate" : "text-3xl"}`}>
        {value ?? (isText ? "—" : 0)}
      </div>
    </Card>
  );
}

function Genres({ title, items = [] }) {
  const counts = items.map(i => i.count ?? i.Count ?? 0);
  const max = Math.max(1, ...counts);

  return (
    <Card>
      <div className="font-semibold mb-3">{title}</div>
      {items.length === 0 ? (
        <div className="text-sm text-gray-500 dark:text-gray-400">—</div>
      ) : (
        <ul className="space-y-2">
          {items.map(i => {
            const genero = i.genero ?? i.Genero;
            const count = i.count ?? i.Count ?? 0;
            const pct = (count / max) * 100;
            return (
              <li key={genero}>
                <div className="flex justify-between text-sm">
                  <span>{genero}</span>
                  <span className="text-gray-500 dark:text-gray-400">{count}</span>
                </div>
                <div className="h-2 rounded mt-1 overflow-hidden bg-gray-200 dark:bg-white/10">
                  <div className="h-full bg-purple-600" style={{ width: `${pct}%` }} />
                </div>
              </li>
            );
          })}
        </ul>
      )}
    </Card>
  );
}

function Favorites({ title, items = [] }) {
  return (
    <Card>
      <div className="font-semibold mb-3">{title}</div>
      {items.length === 0 ? (
        <div className="text-sm text-gray-500 dark:text-gray-400">No tienes favoritos aún.</div>
      ) : (
        <div className="grid grid-cols-2 lg:grid-cols-3 gap-3">
          {items.map(x => (
            <div
              key={x.id}
              className="rounded-lg overflow-hidden border bg-white dark:bg-gray-900/60
                            border-gray-200 dark:border-white/10"
            >
              <div className="aspect-[2/3] bg-gray-100 dark:bg-black">
                <img src={x.imagenUrl} alt={x.titulo} className="w-full h-full object-cover" loading="lazy" />
              </div>
              <div className="p-2">
                <div className="text-sm font-medium truncate">{x.titulo}</div>
                <div className="text-xs text-gray-600 dark:text-gray-400 truncate">
                  {x.genero} · {x.año}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </Card>
  );
}

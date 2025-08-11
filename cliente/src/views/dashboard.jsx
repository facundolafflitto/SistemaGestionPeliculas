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
      .then(r => r.ok ? r.json() : Promise.reject(r.statusText))
      .then(setData)
      .catch(err => console.error("Dashboard error:", err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="p-6">Cargando...</div>;
  if (!data) return <div className="p-6">No se pudo cargar.</div>;

  return (
    <div className="p-6 text-gray-100">
      <h1 className="text-3xl font-bold mb-6">👤 Mi Dashboard</h1>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <div className="bg-gray-800 rounded p-4">
          <div className="text-sm opacity-75">Películas favoritas</div>
          <div className="text-3xl font-semibold">{data.totalPeliculas}</div>
        </div>
        <div className="bg-gray-800 rounded p-4">
          <div className="text-sm opacity-75">Series favoritas</div>
          <div className="text-3xl font-semibold">{data.totalSeries}</div>
        </div>
        <div className="bg-gray-800 rounded p-4">
          <div className="text-sm opacity-75">Usuario</div>
          <div className="text-lg">{data.email}</div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <Card title="Top géneros en películas" items={data.topGenerosPeliculas} />
        <Card title="Top géneros en series" items={data.topGenerosSeries} />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <List title="Tus películas favoritas" items={data.peliculasFavoritas} />
        <List title="Tus series favoritas" items={data.seriesFavoritas} />
      </div>
    </div>
  );
}

function Card({ title, items }) {
  return (
    <div className="bg-gray-800 rounded p-4">
      <h3 className="font-semibold mb-3">{title}</h3>
      <ul className="space-y-2">
        {items.length === 0 ? <li className="opacity-70">—</li> :
          items.map((g) => (
            <li key={g.genero} className="flex justify-between">
              <span>{g.genero}</span>
              <span className="opacity-70">{g.count}</span>
            </li>
          ))}
      </ul>
    </div>
  );
}

function List({ title, items }) {
  return (
    <div className="bg-gray-800 rounded p-4">
      <h3 className="font-semibold mb-3">{title}</h3>
      <div className="grid grid-cols-2 lg:grid-cols-3 gap-3">
        {items.map(x => (
          <div key={x.id} className="bg-gray-900 rounded p-2">
            <img src={x.imagenUrl} alt={x.titulo} className="w-full h-36 object-cover rounded" />
            <div className="mt-2 text-sm font-medium">{x.titulo}</div>
            <div className="text-xs opacity-70">{x.genero} · {x.año}</div>
          </div>
        ))}
      </div>
    </div>
  );
}

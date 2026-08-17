import atasMock from './mockAtas.json';
import colaboradoresMock from './mockColaboradores.json';
import workshopsMock from './mockWorkshops.json';

const BASE_URL = import.meta.env.VITE_API_URL;
const USE_MOCK = import.meta.env.VITE_USE_MOCK === 'true';

class ApiError extends Error {
  constructor(status, detail) {
    super(detail);
    this.status = status;
  }
}

async function get(path, params = {}, signal) {
  const query = new URLSearchParams(
    Object.entries(params).filter(([, v]) => v !== '' && v != null)
  );

  const res = await fetch(`${BASE_URL}${path}?${query}`, { signal });

  if (!res.ok) {
    const problema = await res.json().catch(() => null);
    throw new ApiError(res.status, problema?.detail ?? `Erro ${res.status}`);
  }

  return res.json();
}

function filtrarAtasMock({ workshopNome, data, colaboradorNome }) {
  return atasMock.filter((ata) => {
    const bateWorkshop = !workshopNome ||
      ata.workshop.nome.toLowerCase().includes(workshopNome.toLowerCase());
    const bateData = !data ||
      ata.workshop.dataRealizacao.startsWith(data);
    const bateColaborador = !colaboradorNome ||
      ata.colaboradores.some((c) =>
        c.nome.toLowerCase().includes(colaboradorNome.toLowerCase()));

    return bateWorkshop && bateData && bateColaborador;
  });
}

export async function listarAtas(filtros, signal) {
  if (USE_MOCK) return filtrarAtasMock(filtros);
  return get('/atas', filtros, signal);
}

export async function listarColaboradores(signal) {
  if (USE_MOCK) return colaboradoresMock;
  return get('/colaboradores', {}, signal);
}

export async function listarWorkshops(filtros, signal) {
  if (USE_MOCK) return workshopsMock;
  return get('/workshops', filtros, signal);
}

export { ApiError };

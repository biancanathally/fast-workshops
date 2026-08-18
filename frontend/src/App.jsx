import { useState, useEffect } from 'react';
import { FiltrosBar } from './components/FiltrosBar';
import { ListaAtas } from './components/ListaAtas';
import { DetalheWorkshop } from './components/DetalheWorkshop';
import { EstadoVazio } from './components/EstadoVazio';
import { useDebounce } from './hooks/useDebounce';
import { listarAtas, ApiError } from './services/api';
import './App.css';

export default function App() {
  const [filtros, setFiltros] = useState({ colaboradorNome: '', workshopNome: '', data: '' });
  const [atas, setAtas] = useState([]);
  const [estado, setEstado] = useState('loading'); // 'loading' | 'ok' | 'vazio' | 'erro'
  const [mensagemErro, setMensagemErro] = useState('');
  const [ataSelecionada, setAtaSelecionada] = useState(null);

  const filtrosDebounced = useDebounce(filtros, 300);

  useEffect(() => {
    const controller = new AbortController();

    async function buscar() {
      setEstado('loading');
      try {
        const resultado = await listarAtas(filtrosDebounced, controller.signal);
        setAtas(resultado);
        setEstado(resultado.length === 0 ? 'vazio' : 'ok');
      } catch (err) {
        if (err.name === 'AbortError') return; // requisição cancelada, ignore
        setMensagemErro(err instanceof ApiError ? err.message : 'Erro ao buscar atas.');
        setEstado('erro');
      }
    }

    buscar();
    return () => controller.abort();
  }, [filtrosDebounced]);

  return (
    <main className="app">
      <h1>Atas dos Workshops</h1>

      <FiltrosBar filtros={filtros} aoMudar={setFiltros} />

      {estado === 'ok'
        ? <ListaAtas atas={atas} aoClicarWorkshop={setAtaSelecionada} />
        : <EstadoVazio tipo={estado} mensagem={mensagemErro} />}

      <DetalheWorkshop ata={ataSelecionada} aoFechar={() => setAtaSelecionada(null)} />
    </main>
  );
}

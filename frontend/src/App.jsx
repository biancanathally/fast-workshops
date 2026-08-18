import { useState, useEffect } from 'react';
import { FiltrosBar } from './components/FiltrosBar';
import { ListaAtas } from './components/ListaAtas';
import { DetalheWorkshop } from './components/DetalheWorkshop';
import { EstadoVazio } from './components/EstadoVazio';
import { GraficoBarras } from './components/GraficoBarras';
import { GraficoPizza } from './components/GraficoPizza';
import { useDebounce } from './hooks/useDebounce';
import { listarAtas, listarColaboradores, ApiError } from './services/api';
import './App.css';

export default function App() {
  // Estado da listagem filtrada (a tabela principal)
  const [filtros, setFiltros] = useState({ colaboradorNome: '', workshopNome: '', data: '' });
  const [atas, setAtas] = useState([]);
  const [estado, setEstado] = useState('loading'); // 'loading' | 'ok' | 'vazio' | 'erro'
  const [mensagemErro, setMensagemErro] = useState('');
  const [ataSelecionada, setAtaSelecionada] = useState(null);

  // Estado dos gráficos (visão geral, sem filtro — carregado uma única vez)
  const [colaboradoresTodos, setColaboradoresTodos] = useState([]);
  const [atasTodas, setAtasTodas] = useState([]);
  const [graficosCarregando, setGraficosCarregando] = useState(true);

  const filtrosDebounced = useDebounce(filtros, 300);

  // Efeito 1: busca a listagem principal sempre que os filtros (debounced) mudam
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

  // Efeito 2: busca os dados dos gráficos uma única vez, sem filtro
  useEffect(() => {
    const controller = new AbortController();

    async function buscarDadosGraficos() {
      setGraficosCarregando(true);
      try {
        const [colaboradores, todasAtas] = await Promise.all([
          listarColaboradores(controller.signal),
          listarAtas({}, controller.signal),
        ]);
        setColaboradoresTodos(colaboradores);
        setAtasTodas(todasAtas);
      } catch (err) {
        if (err.name === 'AbortError') return;
        // Falha nos gráficos não deve travar a listagem principal — só loga.
        console.error('Erro ao carregar dados dos gráficos:', err);
      } finally {
        setGraficosCarregando(false);
      }
    }

    buscarDadosGraficos();
    return () => controller.abort();
  }, []); // roda uma vez, ao montar

  return (
    <main className="app">
      <h1>Atas dos Workshops</h1>

      <FiltrosBar filtros={filtros} aoMudar={setFiltros} />

      {estado === 'ok'
        ? <ListaAtas atas={atas} aoClicarWorkshop={setAtaSelecionada} />
        : <EstadoVazio tipo={estado} mensagem={mensagemErro} />}

      <DetalheWorkshop ata={ataSelecionada} aoFechar={() => setAtaSelecionada(null)} />

      <section className="graficos">
        {graficosCarregando ? (
          <p className="estado estado-loading">Carregando gráficos…</p>
        ) : (
          <>
            <GraficoBarras colaboradores={colaboradoresTodos} />
            <GraficoPizza atas={atasTodas} />
          </>
        )}
      </section>
    </main>
  );
}

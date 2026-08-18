import { BarChart, Bar, XAxis, YAxis, Tooltip, LabelList, ResponsiveContainer } from 'recharts';

const LARGURA_POR_BARRA = 56; // px — ajuste este número para deixar as barras mais largas/estreitas

function primeiroNome(nomeCompleto) {
  return nomeCompleto.split(' ')[0];
}

function TooltipBarras({ active, payload }) {
  if (!active || !payload?.length) return null;
  const { nome, workshops } = payload[0].payload;
  return (
    <div style={{
      fontFamily: 'IBM Plex Mono, monospace', fontSize: 12,
      background: '#fff', borderRadius: 8, border: '1px solid #D7DBD0', padding: '6px 10px',
    }}>
      {nome}: {workshops} {workshops === 1 ? 'workshop' : 'workshops'}
    </div>
  );
}

export function GraficoBarras({ colaboradores }) {
  const dados = colaboradores.map((c) => ({
    nome: c.nome,
    nomeCurto: primeiroNome(c.nome),
    workshops: c.totalWorkshops,
  }));

  const largura = dados.length * LARGURA_POR_BARRA;

  return (
    <div className="panel">
      <p className="section-label">Workshops por colaborador</p>

      <div className="grafico-scroll">
        <div style={{ width: `${largura}px`, minWidth: '100%', height: 220 }}>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={dados} margin={{ top: 24, right: 4, left: 4, bottom: 8 }}>
              <XAxis
                dataKey="nomeCurto"
                interval={0}
                tick={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: 10, fill: '#74786D' }}
                axisLine={{ stroke: '#D7DBD0' }}
                tickLine={false}
              />
              <YAxis hide allowDecimals={false} domain={[0, 'dataMax + 1']} />
              <Tooltip content={<TooltipBarras />} cursor={{ fill: '#E3EDE6' }} />
              <Bar dataKey="workshops" fill="#2F6B4F" radius={[3, 3, 0, 0]} maxBarSize={40}>
                <LabelList
                  dataKey="workshops"
                  position="top"
                  style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: 10, fill: '#3C4A41' }}
                />
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}

import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from 'recharts';

const CORES = ['#2F6B4F', '#B8862E', '#7C8B7F', '#C7CDC0', '#4F7A63', '#D0A85C'];

export function GraficoPizza({ atas }) {
  const dados = atas.map((a) => ({ nome: a.workshop.nome, colaboradores: a.totalColaboradores }));

  return (
    <div className="panel">
      <p className="section-label">Colaboradores por workshop</p>
      <div className="donut-wrap">
        <div style={{ width: 96, height: 96, flexShrink: 0 }}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={dados}
                dataKey="colaboradores"
                nameKey="nome"
                innerRadius={28}
                outerRadius={46}
                paddingAngle={2}
                stroke="none"
              >
                {dados.map((_, i) => <Cell key={i} fill={CORES[i % CORES.length]} />)}
              </Pie>
              <Tooltip contentStyle={{
                fontFamily: 'IBM Plex Mono, monospace', fontSize: 12,
                borderRadius: 8, border: '1px solid #D7DBD0',
              }} />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="legend">
          {dados.map((item, i) => (
            <div className="row" key={item.nome}>
              <span className="dot" style={{ background: CORES[i % CORES.length] }} />
              {item.nome}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

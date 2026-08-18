import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';

export function GraficoBarras({ colaboradores }) {
  const dados = colaboradores.map((c) => ({ nome: c.nome, workshops: c.totalWorkshops }));

  return (
    <div className="grafico-container">
      <h3>Workshops por colaborador</h3>
      <ResponsiveContainer width="100%" height={300}>
        <BarChart data={dados}>
          <XAxis dataKey="nome" angle={-30} textAnchor="end" interval={0} height={80} />
          <YAxis allowDecimals={false} />
          <Tooltip />
          <Bar dataKey="workshops" fill="#3b6ea5" radius={[4, 4, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

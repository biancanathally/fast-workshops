import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const CORES = ['#3b6ea5', '#5fa8d3', '#8fc1e3', '#b8dbee', '#2c5282', '#63b3ed'];

export function GraficoPizza({ atas }) {
  const dados = atas.map((a) => ({ nome: a.workshop.nome, colaboradores: a.totalColaboradores }));

  return (
    <div className="grafico-container">
      <h3>Colaboradores por workshop</h3>
      <ResponsiveContainer width="100%" height={300}>
        <PieChart>
          <Pie data={dados} dataKey="colaboradores" nameKey="nome" outerRadius={100} label>
            {dados.map((_, i) => <Cell key={i} fill={CORES[i % CORES.length]} />)}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}

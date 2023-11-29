import { Counter } from "./components/Counter";
import DataHubEstudiante from "./components/DataHubEstudiante";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import TramitesHub from "./components/TramitesHub";
import TramiteFlow from "./components/TramiteFlow";
import RegistroTramite from "./components/RegistroTramite";

const AppRoutes = [
  {
    index: true,
    element: <TramitesHub />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/test',
    element: <TramiteFlow />
  }
];

export default AppRoutes;

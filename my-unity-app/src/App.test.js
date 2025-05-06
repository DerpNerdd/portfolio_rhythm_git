import { render, screen } from '@testing-library/react';
import App from './App';

test('renders loading indicator', () => {
  render(<App />);
  const loadingElement = screen.getByText(/loading/i);
  expect(loadingElement).toBeInTheDocument();
});

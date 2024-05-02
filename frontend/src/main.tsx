import React from "react";
import ReactDOM from "react-dom/client";
import { Amplify } from "aws-amplify";
import { fetchAuthSession } from "aws-amplify/auth";

import { AppRoutes } from "./routes/AppRoutes.tsx";
import { AuthProvider } from "./modules/auth/core/context.tsx";
import { QueryClient, QueryClientProvider } from "react-query";

Amplify.configure(
  {
    Auth: {
      Cognito: {
        userPoolId: import.meta.env.VITE_AWS_USER_POOL_ID,
        userPoolClientId: import.meta.env.VITE_AWS_USER_POOL_CLIENT_ID,
      },
    },
    API: {
      REST: {
        AppApi: {
          endpoint: import.meta.env.VITE_API_HTTP,
        },
      },
    },
  },
  {
    API: {
      REST: {
        headers: async () => {
          const authToken = (
            await fetchAuthSession()
          ).tokens?.idToken?.toString();
          return { Authorization: authToken };
        },
      },
    },
  }
);
const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById("app")!).render(
  <React.StrictMode>
    <AuthProvider>
      <QueryClientProvider client={queryClient}>
        <AppRoutes />
      </QueryClientProvider>
    </AuthProvider>
  </React.StrictMode>
);

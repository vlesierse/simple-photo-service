import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "./core";
import { Box, Container } from "@cloudscape-design/components";

export function Logout() {
  const navigate = useNavigate();
  const { signOut } = useAuth();

  useEffect(() => {
    (async () => {
      await signOut();
      navigate("/auth");
    })();
  }, [signOut, navigate]);
  return (
    <Container header="Signing out">
      <Box variant="p">You are currently being signed out. Please log back in if you would like.</Box>
    </Container>
  );
}

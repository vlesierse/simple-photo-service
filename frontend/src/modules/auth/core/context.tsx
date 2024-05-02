import { FC, createContext, useState, useEffect, useRef } from "react";

import { signOut, getCurrentUser } from "aws-amplify/auth";
import { Hub } from "aws-amplify/utils";
import { WithChildren } from "../../../components";
import { User } from "./types";

type AuthContextProps = {
  currentUser: User | undefined;
  signOut: (global?: boolean) => Promise<void>;
};

const initAuthContextPropsState: AuthContextProps = {
  currentUser: undefined,
  signOut: () => Promise.resolve(),
};

const AuthContext = createContext<AuthContextProps>(initAuthContextPropsState);

const AuthProvider: FC<WithChildren> = ({ children }) => {
  const [currentUser, setCurrentUser] = useState<User | undefined>();

  const context: AuthContextProps = {
    currentUser,
    signOut: async () => {
      await signOut();
    },
  };

  const isMounted = useRef<boolean>(false);
  useEffect(() => {
    if (isMounted.current) {
      (async () => {
        try {
          const user = await getCurrentUser();
          if (user) setCurrentUser(user);
        } catch (err) {
          /* empty */
        }
      })();
    }
    isMounted.current = true;

    return Hub.listen("auth", (data) => {
      const { payload } = data;
      const { event } = payload;
      switch (event) {
        case "signedIn":
          setCurrentUser(payload.data);
          break;
        case "signedOut":
          setCurrentUser(undefined);
          break;
        default:
          break;
      }
    });
  }, []);

  return (
    <AuthContext.Provider value={context}>{children}</AuthContext.Provider>
  );
};

export { AuthProvider, AuthContext, type AuthContextProps };

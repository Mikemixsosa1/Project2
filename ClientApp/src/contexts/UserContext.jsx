import React, { createContext, useContext, useState } from 'react';

const UserContext = createContext(null);

export const useUser = () => useContext(UserContext);

export const UserProvider = ({ children }) => {
    const [userType, setUserType] = useState(null);

    return (
        <UserContext.Provider value={{ userType, setUserType }}>
            {children}
        </UserContext.Provider>
    );
};

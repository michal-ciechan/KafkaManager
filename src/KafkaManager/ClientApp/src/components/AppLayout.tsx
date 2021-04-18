import React, {useState} from 'react';
import {Layout} from 'antd';
import {MenuFoldOutlined, MenuUnfoldOutlined,} from '@ant-design/icons';
import {NavMenu} from "./NavMenu";

const {Header, Content} = Layout;


export function AppLayout(props: React.PropsWithChildren<{}>) {
    // Create the collapsed state.
    const [collapsed, setCollapsed] = useState(false);

    return (
        <Layout style={{height: "100vh"}}>
            <NavMenu collapsed={collapsed}/>
            <Layout className="site-layout">
                <Header className="site-layout-background" style={{padding: 0}}>
                    {React.createElement(collapsed ? MenuUnfoldOutlined : MenuFoldOutlined, {
                        className: 'trigger',
                        onClick: () => setCollapsed(!collapsed),
                    })}
                </Header>
                <Content
                    className="site-layout-background"
                    style={{
                        margin: '24px 16px',
                        padding: 24,
                        minHeight: "280",
                    }}
                >
                    {props.children}
                </Content>
            </Layout>
        </Layout>
    );
}

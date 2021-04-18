import React from 'react';
import {Layout, Menu} from 'antd';
import {UploadOutlined, UserOutlined, VideoCameraOutlined,} from '@ant-design/icons';
import {NavLink} from "react-router-dom";

const {Sider} = Layout;

interface Props {
    collapsed: boolean
}

export function NavMenu(props: Props) {
    return (
        <Sider trigger={null} collapsible collapsed={props.collapsed}>
            <div className="logo"/>
            <Menu theme="dark" mode="inline" defaultSelectedKeys={['1']}>
                <Menu.Item key="1" icon={<UserOutlined/>}>
                    <NavLink to="/topic">Topics</NavLink>
                </Menu.Item>
                <Menu.Item key="2" icon={<VideoCameraOutlined/>}>
                    <NavLink to="/nav2">NAV 2</NavLink>
                </Menu.Item>
                <Menu.Item key="3" icon={<UploadOutlined/>}>
                    <NavLink to="/nav3">NAV 3</NavLink>
                </Menu.Item>
            </Menu>
        </Sider>
    )
}

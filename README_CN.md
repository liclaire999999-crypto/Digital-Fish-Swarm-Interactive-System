# 数字鱼群手势交互系统

基于 Unity3D 的数字鱼群手势交互系统设计与实现

## 项目概述

本项目是一个结合数字媒体艺术、Unity 实时渲染、群体智能算法和 AI 视觉交互的沉浸式数字艺术交互作品。

### 核心功能

- **Boids 鱼群模拟系统** — 实现聚集、分离、对齐、惊散四大群体行为
- **实时手势交互** — 基于 MediaPipe 的手部识别和追踪
- **视觉特效系统** — Shader 发光效果、粒子系统拖尾
- **性能优化** — 支持 100+ 鱼群，≥30 FPS

## 项目结构

```
Digital-Fish-Swarm-Interactive-System/
├── README_CN.md                    # 中文说明
├── README_EN.md                    # 英文说明
├── Assets/
│   ├── Scripts/
│   │   ├── Boids/                 # Boids 算法核心
│   │   ├── Fish/                  # 鱼模型与动画
│   │   ├── Interaction/           # 手势交互模块
│   │   └── Manager/               # 系统管理器
│   ├── Materials/                 # Shader 材质
│   ├── Prefabs/                   # 预制体
│   ├── Scenes/                    # 场景
│   ├── Shaders/                   # Shader 脚本
│   └── Resources/                 # 资源配置
├── Docs/                          # 文档
└── .gitignore                     # Git 忽略文件
```

## 开发进度

- [ ] 基础目录结构
- [ ] Boids 算法实现
- [ ] 鱼模型与预制体
- [ ] 手势识别集成
- [ ] 视觉特效
- [ ] 性能优化
- [ ] 测试与打包

## 技术栈

- **引擎**: Unity 2022.3 LTS
- **语言**: C#
- **手势识别**: MediaPipe
- **渲染**: Shader Graph
- **特效**: Particle System, VFX Graph

## 环境配置

详见 `Docs/SETUP.md`

## 许可证

MIT

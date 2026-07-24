# 项目规格说明书 | Project Specification

## 核心需求 | Core Requirements

### 功能需求 | Functional Requirements

#### 1. Boids 鱼群模拟系统
- [ ] 聚集行为 (Cohesion) - 鱼群自动靠近形成群体
- [ ] 分离行为 (Separation) - 避免鱼之间碰撞
- [ ] 对齐行为 (Alignment) - 保持游动方向一致
- [ ] 惊散行为 (Escape/Flee) - 由用户交互触发

#### 2. 手势交互
- [ ] 手部接近检测 - 触发鱼群逃散
- [ ] 手部移动追踪 - 鱼群跟随
- [ ] 手势识别 - 挥动产生扰动

#### 3. 视觉效果
- [ ] Fish 发光效果 (Shader Graph)
- [ ] 半透明材质
- [ ] 拖尾效果 (Particle System)
- [ ] 水下光影

### 性能需求 | Performance Requirements

- 鱼群数量: ≥ 100 条
- 帧率: ≥ 30 FPS
- 运行稳定性: 无明显卡顿

### 系统架构 | System Architecture

```
数字鱼群交互系统
├── 鱼群管理模块 (Fish Manager)
├── Boids 算法模块
│   ├── 聚集
│   ├── 分离
│   ├── 对齐
│   └── 惊散
├── 手势识别模块
│   ├── 摄像头输入
│   ├── 手掌位置
│   └── 手势判断
├── 鱼群控制模块
├── Shader 视觉模块
│   ├── 发光
│   ├── 半透明
│   └── 水波
└── 粒子特效模块
    ├── 拖尾
    └── 环境效果
```

## 技术规格 | Technical Specifications

- 开发平台: Unity 2022.3 LTS
- 开发语言: C#
- 渲染管线: Universal RP (推荐)
- 物理引擎: Physics-based or Custom

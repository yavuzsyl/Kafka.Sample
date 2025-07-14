# Kafka Learning Sample Project

This repository contains various .NET examples and experiments for learning Apache Kafka concepts, including producers, consumers, consumer groups, partitions, and clustering.

## 🏗️ Project Structure

```
├── Order.API/                     # Web API for creating orders (publishes events)
├── Kafka.Producer/                # Standalone producer examples
├── Kafka.Consumer/                # Standalone consumer examples  
├── Kafka.ConsumerMultipleGroupTest/ # Multiple consumer group testing
├── Shared.Events/                 # Shared event models and constants
├── docker-compose.yml             # Single broker + UI setup
├── docker-compose-cluster.yml     # 3-broker cluster + UI setup
└── kafka-consumer-groups-and-partitions # Consumer group documentation
```

## 🐳 Docker Configurations

### Single Broker Setup (`docker-compose.yml`)

This configuration sets up a simple Kafka environment with:
- **1 Kafka broker** (KRaft mode, no Zookeeper needed)
- **Kafka UI** for visual management and monitoring
- **Ports:**
  - `9094`: Kafka broker (external access)
  - `8080`: Kafka UI web interface

```bash
# Start single broker
docker-compose up -d

# Stop
docker-compose down
```

**Access Points:**
- Kafka UI: http://localhost:8080
- Kafka broker: `localhost:9094`

### Multi-Broker Cluster Setup (`docker-compose-cluster.yml`)

This configuration sets up a production-like cluster with:
- **3 Kafka brokers** (kafka-0, kafka-1, kafka-2)
- **High availability** with replication factor 3
- **Kafka UI** for cluster monitoring
- **Ports:**
  - `7000`: kafka-0 (external access)
  - `7001`: kafka-1 (external access)
  - `7002`: kafka-2 (external access)
  - `8080`: Kafka UI web interface

```bash
# Start cluster
docker-compose -f docker-compose-cluster.yml up -d

# Stop cluster
docker-compose -f docker-compose-cluster.yml down
```

- **Replication Factor**: 3 (data replicated across all brokers)

## 🚀 Getting Started

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK
- Visual Studio or VS Code

### Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Kafka.Sample
   ```

2. **Start Kafka (choose one)**
   ```bash
   # Single broker (easier for learning)
   docker-compose up -d
   
   # OR multi-broker cluster (production-like)
   docker-compose -f docker-compose-cluster.yml up -d
   ```

3. **Open Kafka UI**
   - Navigate to http://localhost:8080
   - Explore topics, partitions, and consumer groups

4. **Build the solution**
   ```bash
   dotnet build
   ```

## 📚 Learning Modules

### 1. Order API + Event Publishing
**Project**: `Order.API`

- Creates orders via HTTP POST
- Publishes `OrderCreatedEvent` to Kafka topic
- Shows integration between web API and Kafka


### 2. Producer Examples
**Project**: `Kafka.Producer`

Demonstrates various producer patterns:
- Messages with null keys
- Messages with integer keys
- Complex message types with headers
- Partition-specific message sending
- Producer acknowledgment modes (acks)
- Retry mechanisms
- Cluster topic creation


**Key Concepts Covered:**
- **Message Keys**: Null, integer, and complex keys
- **Partitioning**: Specific partition targeting
- **Acknowledgments**: `acks=0`, `acks=1`, `acks=all`
- **Retries**: Automatic retry on failures
- **Headers**: Custom message metadata

### 3. Consumer Examples
**Project**: `Kafka.Consumer`

Demonstrates various consumer patterns:
- Basic message consumption
- Key-based message handling
- Header processing
- Partition-specific consumption
- Offset management
- Cluster consumer groups


### 4. Consumer Groups & Partitions
**Project**: `Kafka.ConsumerMultipleGroupTest`

Explores advanced consumer group concepts:
- Multiple consumer groups reading same topic
- Partition assignment within groups
- Load balancing across consumers
- Consumer rebalancing

See `kafka-consumer-groups-and-partitions` for detailed explanation.

## 🎯 Key Kafka Concepts Demonstrated

### Producer Acknowledgments (acks)
- **`acks=0`**: Fire-and-forget (lowest latency, no delivery guarantee)
- **`acks=1`**: Wait for leader confirmation (balanced durability/latency)
- **`acks=all`**: Wait for all replicas (highest durability, highest latency)

### Consumer Groups
- **Isolation**: Different groups read independently
- **Partition Assignment**: One partition per consumer within a group
- **Rebalancing**: Automatic partition reassignment on consumer changes
- **Scaling**: Consumers ≈ Partitions for optimal performance

### Message Ordering
- **Per-Partition**: Messages within a partition are ordered
- **Cross-Partition**: No ordering guarantee across partitions
- **Key-Based**: Messages with same key go to same partition

### Fault Tolerance (Cluster Mode)
- **Replication**: Data copied across multiple brokers
- **Leader Election**: Automatic failover on broker failure
- **Durability**: Configurable minimum in-sync replicas

## 🛠️ Development Tips

### Useful Docker Commands
```bash
# Clean up volumes (removes all data)
docker-compose down -v

# Scale consumers in cluster mode
docker-compose -f docker-compose-cluster.yml up -d --scale kafka-consumer=3
```


## 🔧 Configuration Files

### Topic Configuration Examples
- **Retention**: Messages kept for specified time
- **Partitions**: Parallel processing units
- **Replication**: Data redundancy across brokers
- **Cleanup Policy**: Delete vs compact

### Consumer Configuration
- **Group ID**: Consumer group identifier
- **Auto Offset Reset**: `earliest` vs `latest`
- **Enable Auto Commit**: Automatic offset management
- **Session Timeout**: Consumer failure detection

## 📖 Learning Path

1. **Start Simple**: Use single broker setup
2. **Explore UI**: Get familiar with Kafka UI
3. **Run Producer**: Send messages and observe in UI
4. **Run Consumer**: Consume messages and understand groups
5. **Experiment**: Try different configurations
6. **Scale Up**: Move to cluster setup
7. **Test Resilience**: Stop brokers and observe behavior


## 📚 Additional Resources

- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent Kafka .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
- [Kafka UI Documentation](https://docs.kafka-ui.provectus.io/)

---

Happy learning! 🚀 

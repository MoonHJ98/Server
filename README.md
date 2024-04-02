게임 서버의 아키텍처

![image](https://github.com/MoonHJ98/Server/assets/91938770/cb170dc0-10b9-4ed8-9c3f-51a5f1ea7162)

주요 기능 설명

Account Server
1. 유저 로그인
2. DB에서 아이디 확인, 유저 데이터 로드 후 Zone Server로 이동

Zone Server
1. 유저 상태 동기화
2. 유저, 몬스터 상태 관리
3. 유저 상태, 아이템 DB저장
4. 다른 유저의 채팅 전달

Matching Server
1. 매칭된 유저 관리
2. 던전 서버로 이동

Chatting Server
1. 클라이언트에서 입력한 채팅을 존, 던전 서버로 전달

Dungeon Server
1. 다른 유저의 채팅을 전달

특징
1. 스레드
Zone Server의 원활한 작동을 위해 스레드를 여러 개로 나눴다

    ① Receive Packet(N개)
    .net Core의 Thread Pool에서 관리한다. N은 입장한 유저 수이다.

    ② Game Logic
    게임의 로직의 돌아가는 스레드이다. 유저와 몬스터의 상태를 관리한다.

    ③ Send Packet
    여러 클라이언트에서 보낸 패킷을 모아두다가 전달하는 스레드이다.
    순서를 유지하기 위해 Queue를 사용한다.

    ④ DB
    DB에 접근에 데이터를 읽고 쓰는 스레드이다.
    순서를 유지하기 위해 Queue를 사용한다.
    모든 DB접근은 비동기로 작동되도록 구현했다.

    ⑤ Chatting
    Chatting Server에서 전달된 채팅을 Zone, Dungeon 내의 모든 유저에게 전달한다.

2. 이동 패킷 최적화
클라이언트에서 이동키를 입력하면 캐릭터 바로 앞으로 이동하는 방식이기 때문에 이동 패킷을 전달하는 것에 대해 최적화가 필요했다.
프레임마다 이동패킷을 전달하는 것이 아니라 추측항법을 사용해 이동패킷 전달에 최적화를 하였고, 0.25초마다 클라이언트의 위치를 보내어 서버에 보내어 업데이트하도록 구현했다.

import { Server, Socket } from "socket.io";

export interface Player{
    socket: Socket,
    uid: string,
    nick: string
}

export interface Room{
    player1: Player;
    player2: Player | null;
    timestamp: number
}


export interface Action{
    to: string,
    action: string,
    from_pos: string,
    to_pos: string,
    nfs: string,
    round: string
}


export interface PlacementInfo{
    to: string,
    placed_list: string
}

export interface JoinPrivateReq{
    uid: string,
    v: string,
    mid: string,
    nick: string
}

export interface MatchEndInfo{
    uid: string,
    status: string,
    match_id: string,
}

export interface ID_Nick{
    uid: string,
    v: string,
    nick: string
}

export interface joinTournamentInfo{
    uid: string,
    match_id: string
}
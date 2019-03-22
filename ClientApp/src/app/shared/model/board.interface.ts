interface Board {
    id: string;
    title: string;
    lists: Array<CardList>;
    members: Array<BoardMember>;
    background: string;
}

PGDMP                         {         	   dogsclub1    15.2    15.2 N    Q           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            R           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            S           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            T           1262    36321 	   dogsclub1    DATABASE     }   CREATE DATABASE dogsclub1 WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE dogsclub1;
                postgres    false            �            1255    36322    archive_dead_dogs() 	   PROCEDURE     �  CREATE PROCEDURE public.archive_dead_dogs()
    LANGUAGE plpgsql
    AS $$
DECLARE
  current_id INT;
  related_dog INT;
  child BOOLEAN := FALSE;
BEGIN
  FOR current_id IN SELECT id_dog FROM dogs WHERE death_date IS NOT NULL LOOP
    RAISE NOTICE 'Проверка собаки с id %', current_id;
    FOR related_dog IN SELECT id_dog FROM dogs WHERE father_id = current_id OR mother_id = current_id  LOOP
      RAISE NOTICE 'Собака с related_dog % имеет родителя current_id %', related_dog, current_id;
      child := TRUE;
    END LOOP;
    IF child = FALSE THEN 
      -- Проверяем, существует ли запись с таким же id_dog в таблице archive
      IF NOT EXISTS(SELECT 1 FROM archive WHERE id_dog = current_id) THEN
        INSERT INTO archive (id_dog, nickname, owner_id, birth_date, death_date, gender, breed, father_id, mother_id, exhibition_id)
        SELECT id_dog, nickname, owner_id, birth_date, death_date, gender, breed, father_id, mother_id, exhibition_id
        FROM dogs
        WHERE id_dog = current_id;
      END IF;
      -- Удаляем запись из dogs
      --DELETE FROM dogs WHERE id_dog = current_id;
    END IF;
  END LOOP;
END;
$$;
 +   DROP PROCEDURE public.archive_dead_dogs();
       public          postgres    false            �            1255    36323    award_date_check()    FUNCTION     �  CREATE FUNCTION public.award_date_check() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
            BEGIN
              IF NEW.date_of_receiving IS NULL THEN
                NEW.date_of_receiving := CURRENT_DATE;
              ELSEIF NEW.date_of_receiving > CURRENT_DATE THEN
                RAISE EXCEPTION 'Дата получения награды не может быть больше текущей даты';
              END IF;
              RETURN NEW;
            END;
            $$;
 )   DROP FUNCTION public.award_date_check();
       public          postgres    false            �            1255    36324    check_breed()    FUNCTION     _  CREATE FUNCTION public.check_breed() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
                DECLARE 
                    mother_breed TEXT;
                    father_breed TEXT;
                BEGIN
                    SELECT breed INTO mother_breed FROM dogs WHERE id_dog = NEW.mother_id;
                    SELECT breed INTO father_breed FROM dogs WHERE id_dog = NEW.father_id;
                    IF mother_breed = father_breed THEN
                        UPDATE dogs
                        SET breed = mother_breed
                        WHERE id_dog = NEW.id_dog;
                    ELSE
                        UPDATE dogs
                        SET breed = 'метис'
                        WHERE id_dog = NEW.id_dog;
                    END IF;
                    RETURN NEW;
                END;
                $$;
 $   DROP FUNCTION public.check_breed();
       public          postgres    false            �            1255    36325 &   format_phone_number(character varying)    FUNCTION     �  CREATE FUNCTION public.format_phone_number(phone_number character varying) RETURNS character varying
    LANGUAGE plpgsql
    AS $$
DECLARE
    formatted_phone VARCHAR := '';
BEGIN
    -- Удаляем все символы, кроме цифр
    phone_number := regexp_replace(phone_number, '[^0-9]+', '', 'g');

    IF length(phone_number) = 7 THEN
        -- Если номер содержит 7 цифр, разделяем их двумя дефисами
        formatted_phone := substr(phone_number, 1, 3) || '-' || substr(phone_number, 4, 2) || '-' || substr(phone_number, 6, 2);
    ELSIF length(phone_number) = 10 THEN
        -- Если номер содержит 10 цифр, добавляем в начало '8-', затем разделяем двумя дефисами
        formatted_phone := '8-' || substr(phone_number, 1, 3) || '-' || substr(phone_number, 4, 2) || '-' || substr(phone_number, 6, 2) || '-' || substr(phone_number, 8, 2) || '-' || substr(phone_number, 10, 2);
    ELSIF length(phone_number) = 11 THEN
        -- Если номер содержит 11 цифр, разделяем их двумя дефисами
        formatted_phone := substr(phone_number, 1, 1) || '-' || substr(phone_number, 2, 3) || '-' || substr(phone_number, 5, 3) || '-' || substr(phone_number, 7, 2) || '-' || substr(phone_number, 9, 2);
    END IF;

    RETURN formatted_phone;
END;
$$;
 J   DROP FUNCTION public.format_phone_number(phone_number character varying);
       public          postgres    false            �            1255    36326 &   get_candidates(text, integer, integer)    FUNCTION     �  CREATE FUNCTION public.get_candidates(breed_ text, awards_count_ integer, age_ integer) RETURNS TABLE(dog_id integer, nickname text, age integer, awards_count integer, owner_fullname text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        dogs.id_dog,
        dogs.nickname,
        EXTRACT(YEAR FROM age(current_date, dogs.birth_date)) AS age,
        COUNT(awards.id_award) AS awards_count,
        owners.last_name || ' ' || owners.first_name || ' ' || owners.middle_name AS owner_fullname
    FROM
        dogs
    LEFT JOIN
        owners ON dogs.owner_id = owners.owner_id
    LEFT JOIN
        awards ON dogs.id_dog = awards.id_dog
    WHERE
        dogs.breed = breed_
        AND EXTRACT(YEAR FROM age(current_date, dogs.birth_date)) > age_
    GROUP BY
        dogs.id_dog,
        dogs.nickname,
        owners.last_name,
        owners.first_name,
        owners.middle_name
    HAVING
        COUNT(awards.id_award) >= awards_count_;
END;
$$;
 W   DROP FUNCTION public.get_candidates(breed_ text, awards_count_ integer, age_ integer);
       public          postgres    false            �            1255    36327    get_dog_age(date, date)    FUNCTION     �  CREATE FUNCTION public.get_dog_age(dog_birth_date date, date_to date DEFAULT CURRENT_DATE) RETURNS text
    LANGUAGE plpgsql
    AS $$
    DECLARE
        years INT;
        months INT;
    BEGIN
        SELECT 
            EXTRACT(YEAR FROM age(date_to, dog_birth_date)),
            EXTRACT(MONTH FROM age(date_to, dog_birth_date))
        INTO 
            years,
            months;
        RETURN years || ' год(а)(лет) ' || months || ' месяц(ев)';
    END;
    $$;
 E   DROP FUNCTION public.get_dog_age(dog_birth_date date, date_to date);
       public          postgres    false            �            1255    36328 $   get_dog_info(text, integer, integer)    FUNCTION     �  CREATE FUNCTION public.get_dog_info(breed_ text, years_ integer, awards_count_ integer) RETURNS TABLE(id_dog integer, age interval, awards_count bigint, breed text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    date_now DATE := current_date;
BEGIN
    RETURN QUERY
    SELECT dogs.id_dog, (date_now - dogs.birth_date) AS age, COUNT(exhibition.id_dog) AS awards_count, dogs.breed
    FROM dogs
    LEFT JOIN exhibition ON dogs.id_dog = exhibition.id_dog
    WHERE EXTRACT(YEAR FROM age(date_now, dogs.birth_date)) > years_ AND dogs.breed = breed_
    GROUP BY dogs.id_dog, dogs.birth_date, dogs.breed
    HAVING COUNT(exhibition.id_dog) >= awards_count_;
END;
$$;
 W   DROP FUNCTION public.get_dog_info(breed_ text, years_ integer, awards_count_ integer);
       public          postgres    false            �            1255    36329     select_dogs_for_exhibition(text) 	   PROCEDURE     t  CREATE PROCEDURE public.select_dogs_for_exhibition(IN breed_ text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    date_now date := current_date;
    dog_breed TEXT := breed_;
    participants record;
BEGIN
    FOR participants IN SELECT dogs.id_dog, (date_now - dogs.birth_date) AS age, COUNT(exhibition.id_dog) AS awards_count, dogs.breed
        FROM dogs
        LEFT JOIN exhibition ON dogs.id_dog = exhibition.id_dog
        WHERE EXTRACT(year FROM age(date_now, dogs.birth_date)) > 3 AND dogs.breed = dog_breed
        GROUP BY dogs.id_dog, dogs.birth_date, dogs.breed
        HAVING COUNT(exhibition.id_dog) >= 3
    LOOP
        -- Выводим информацию о кандидате
        RAISE NOTICE 'Dog ID: %, Age: %, Awards Count: %, Breed: %',
            participants.id_dog, participants.age, participants.awards_count, participants.breed;
    END LOOP;
END;
$$;
 B   DROP PROCEDURE public.select_dogs_for_exhibition(IN breed_ text);
       public          postgres    false            �            1255    36330    shorten_name(text)    FUNCTION     	  CREATE FUNCTION public.shorten_name(full_name text) RETURNS text
    LANGUAGE plpgsql
    AS $$
                BEGIN
                    RETURN regexp_replace(full_name, '(\S+)\s+(\S)\S*\s+(\S)\S*', '\1 \2. \3.', 'g');
                END;
                $$;
 3   DROP FUNCTION public.shorten_name(full_name text);
       public          postgres    false            �            1259    36331    __EFMigrationsHistory    TABLE     �   CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
 +   DROP TABLE public."__EFMigrationsHistory";
       public         heap    postgres    false            �            1259    36334    archive    TABLE     Y  CREATE TABLE public.archive (
    id_dog integer NOT NULL,
    nickname text NOT NULL,
    owner_id integer NOT NULL,
    birth_date timestamp with time zone NOT NULL,
    death_date timestamp with time zone,
    gender text NOT NULL,
    breed text NOT NULL,
    father_id integer,
    mother_id integer,
    exhibition_id integer DEFAULT 0
);
    DROP TABLE public.archive;
       public         heap    postgres    false            �            1259    36340    archive_id_dog_seq    SEQUENCE     �   CREATE SEQUENCE public.archive_id_dog_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 )   DROP SEQUENCE public.archive_id_dog_seq;
       public          postgres    false    215            U           0    0    archive_id_dog_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public.archive_id_dog_seq OWNED BY public.archive.id_dog;
          public          postgres    false    216            �            1259    36341    auth    TABLE     �   CREATE TABLE public.auth (
    id integer NOT NULL,
    login text NOT NULL,
    password text NOT NULL,
    role integer NOT NULL
);
    DROP TABLE public.auth;
       public         heap    postgres    false            �            1259    36346    auth_id_seq    SEQUENCE     �   CREATE SEQUENCE public.auth_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 "   DROP SEQUENCE public.auth_id_seq;
       public          postgres    false    217            V           0    0    auth_id_seq    SEQUENCE OWNED BY     ;   ALTER SEQUENCE public.auth_id_seq OWNED BY public.auth.id;
          public          postgres    false    218            �            1259    36347    awards    TABLE     �   CREATE TABLE public.awards (
    id_award integer NOT NULL,
    id_dog integer NOT NULL,
    name_of_award text NOT NULL,
    date_of_receiving timestamp with time zone
);
    DROP TABLE public.awards;
       public         heap    postgres    false            �            1259    36352    awards_id_award_seq    SEQUENCE     �   CREATE SEQUENCE public.awards_id_award_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.awards_id_award_seq;
       public          postgres    false    219            W           0    0    awards_id_award_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.awards_id_award_seq OWNED BY public.awards.id_award;
          public          postgres    false    220            �            1259    36353    dogs    TABLE     L  CREATE TABLE public.dogs (
    id_dog integer NOT NULL,
    nickname text NOT NULL,
    owner_id integer NOT NULL,
    birth_date timestamp with time zone NOT NULL,
    death_date timestamp with time zone,
    gender text NOT NULL,
    breed text NOT NULL,
    father_id integer,
    mother_id integer,
    exhibition_id integer
);
    DROP TABLE public.dogs;
       public         heap    postgres    false            �            1259    36358    dogs_id_dog_seq    SEQUENCE     �   CREATE SEQUENCE public.dogs_id_dog_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 &   DROP SEQUENCE public.dogs_id_dog_seq;
       public          postgres    false    221            X           0    0    dogs_id_dog_seq    SEQUENCE OWNED BY     C   ALTER SEQUENCE public.dogs_id_dog_seq OWNED BY public.dogs.id_dog;
          public          postgres    false    222            �            1259    36359 
   exhibition    TABLE     �   CREATE TABLE public.exhibition (
    id_exhibition integer NOT NULL,
    id_dog integer NOT NULL,
    name_exhibition text NOT NULL,
    start_date timestamp with time zone NOT NULL,
    end_date timestamp with time zone
);
    DROP TABLE public.exhibition;
       public         heap    postgres    false            �            1259    36364    exhibition_id_exhibition_seq    SEQUENCE     �   CREATE SEQUENCE public.exhibition_id_exhibition_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 3   DROP SEQUENCE public.exhibition_id_exhibition_seq;
       public          postgres    false    223            Y           0    0    exhibition_id_exhibition_seq    SEQUENCE OWNED BY     ]   ALTER SEQUENCE public.exhibition_id_exhibition_seq OWNED BY public.exhibition.id_exhibition;
          public          postgres    false    224            �            1259    36365    owners    TABLE     �   CREATE TABLE public.owners (
    owner_id integer NOT NULL,
    last_name text NOT NULL,
    first_name text NOT NULL,
    middle_name text NOT NULL,
    address text NOT NULL,
    phone text NOT NULL
);
    DROP TABLE public.owners;
       public         heap    postgres    false            �            1259    36370    owners_owner_id_seq    SEQUENCE     �   CREATE SEQUENCE public.owners_owner_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.owners_owner_id_seq;
       public          postgres    false    225            Z           0    0    owners_owner_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.owners_owner_id_seq OWNED BY public.owners.owner_id;
          public          postgres    false    226            �           2604    36371    archive id_dog    DEFAULT     p   ALTER TABLE ONLY public.archive ALTER COLUMN id_dog SET DEFAULT nextval('public.archive_id_dog_seq'::regclass);
 =   ALTER TABLE public.archive ALTER COLUMN id_dog DROP DEFAULT;
       public          postgres    false    216    215            �           2604    36372    auth id    DEFAULT     b   ALTER TABLE ONLY public.auth ALTER COLUMN id SET DEFAULT nextval('public.auth_id_seq'::regclass);
 6   ALTER TABLE public.auth ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    217            �           2604    36373    awards id_award    DEFAULT     r   ALTER TABLE ONLY public.awards ALTER COLUMN id_award SET DEFAULT nextval('public.awards_id_award_seq'::regclass);
 >   ALTER TABLE public.awards ALTER COLUMN id_award DROP DEFAULT;
       public          postgres    false    220    219            �           2604    36374    dogs id_dog    DEFAULT     j   ALTER TABLE ONLY public.dogs ALTER COLUMN id_dog SET DEFAULT nextval('public.dogs_id_dog_seq'::regclass);
 :   ALTER TABLE public.dogs ALTER COLUMN id_dog DROP DEFAULT;
       public          postgres    false    222    221            �           2604    36375    exhibition id_exhibition    DEFAULT     �   ALTER TABLE ONLY public.exhibition ALTER COLUMN id_exhibition SET DEFAULT nextval('public.exhibition_id_exhibition_seq'::regclass);
 G   ALTER TABLE public.exhibition ALTER COLUMN id_exhibition DROP DEFAULT;
       public          postgres    false    224    223            �           2604    36376    owners owner_id    DEFAULT     r   ALTER TABLE ONLY public.owners ALTER COLUMN owner_id SET DEFAULT nextval('public.owners_owner_id_seq'::regclass);
 >   ALTER TABLE public.owners ALTER COLUMN owner_id DROP DEFAULT;
       public          postgres    false    226    225            B          0    36331    __EFMigrationsHistory 
   TABLE DATA           R   COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
    public          postgres    false    214   �t       C          0    36334    archive 
   TABLE DATA           �   COPY public.archive (id_dog, nickname, owner_id, birth_date, death_date, gender, breed, father_id, mother_id, exhibition_id) FROM stdin;
    public          postgres    false    215   �u       E          0    36341    auth 
   TABLE DATA           9   COPY public.auth (id, login, password, role) FROM stdin;
    public          postgres    false    217   /v       G          0    36347    awards 
   TABLE DATA           T   COPY public.awards (id_award, id_dog, name_of_award, date_of_receiving) FROM stdin;
    public          postgres    false    219   `v       I          0    36353    dogs 
   TABLE DATA           �   COPY public.dogs (id_dog, nickname, owner_id, birth_date, death_date, gender, breed, father_id, mother_id, exhibition_id) FROM stdin;
    public          postgres    false    221   Jw       K          0    36359 
   exhibition 
   TABLE DATA           b   COPY public.exhibition (id_exhibition, id_dog, name_exhibition, start_date, end_date) FROM stdin;
    public          postgres    false    223   �y       M          0    36365    owners 
   TABLE DATA           ^   COPY public.owners (owner_id, last_name, first_name, middle_name, address, phone) FROM stdin;
    public          postgres    false    225   �z       [           0    0    archive_id_dog_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.archive_id_dog_seq', 16, true);
          public          postgres    false    216            \           0    0    auth_id_seq    SEQUENCE SET     :   SELECT pg_catalog.setval('public.auth_id_seq', 1, false);
          public          postgres    false    218            ]           0    0    awards_id_award_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.awards_id_award_seq', 36, true);
          public          postgres    false    220            ^           0    0    dogs_id_dog_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.dogs_id_dog_seq', 70, true);
          public          postgres    false    222            _           0    0    exhibition_id_exhibition_seq    SEQUENCE SET     K   SELECT pg_catalog.setval('public.exhibition_id_exhibition_seq', 10, true);
          public          postgres    false    224            `           0    0    owners_owner_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.owners_owner_id_seq', 10, true);
          public          postgres    false    226            �           2606    36378 .   __EFMigrationsHistory PK___EFMigrationsHistory 
   CONSTRAINT     {   ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
 \   ALTER TABLE ONLY public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory";
       public            postgres    false    214            �           2606    36380    archive PK_archive 
   CONSTRAINT     V   ALTER TABLE ONLY public.archive
    ADD CONSTRAINT "PK_archive" PRIMARY KEY (id_dog);
 >   ALTER TABLE ONLY public.archive DROP CONSTRAINT "PK_archive";
       public            postgres    false    215            �           2606    36382    auth PK_auth 
   CONSTRAINT     L   ALTER TABLE ONLY public.auth
    ADD CONSTRAINT "PK_auth" PRIMARY KEY (id);
 8   ALTER TABLE ONLY public.auth DROP CONSTRAINT "PK_auth";
       public            postgres    false    217            �           2606    36384    awards PK_awards 
   CONSTRAINT     V   ALTER TABLE ONLY public.awards
    ADD CONSTRAINT "PK_awards" PRIMARY KEY (id_award);
 <   ALTER TABLE ONLY public.awards DROP CONSTRAINT "PK_awards";
       public            postgres    false    219            �           2606    36386    dogs PK_dogs 
   CONSTRAINT     P   ALTER TABLE ONLY public.dogs
    ADD CONSTRAINT "PK_dogs" PRIMARY KEY (id_dog);
 8   ALTER TABLE ONLY public.dogs DROP CONSTRAINT "PK_dogs";
       public            postgres    false    221            �           2606    36388    exhibition PK_exhibition 
   CONSTRAINT     c   ALTER TABLE ONLY public.exhibition
    ADD CONSTRAINT "PK_exhibition" PRIMARY KEY (id_exhibition);
 D   ALTER TABLE ONLY public.exhibition DROP CONSTRAINT "PK_exhibition";
       public            postgres    false    223            �           2606    36390    owners PK_owners 
   CONSTRAINT     V   ALTER TABLE ONLY public.owners
    ADD CONSTRAINT "PK_owners" PRIMARY KEY (owner_id);
 <   ALTER TABLE ONLY public.owners DROP CONSTRAINT "PK_owners";
       public            postgres    false    225            �           1259    36391    IX_archive_exhibition_id    INDEX     W   CREATE INDEX "IX_archive_exhibition_id" ON public.archive USING btree (exhibition_id);
 .   DROP INDEX public."IX_archive_exhibition_id";
       public            postgres    false    215            �           1259    36392    IX_archive_father_id    INDEX     O   CREATE INDEX "IX_archive_father_id" ON public.archive USING btree (father_id);
 *   DROP INDEX public."IX_archive_father_id";
       public            postgres    false    215            �           1259    36393    IX_archive_mother_id    INDEX     O   CREATE INDEX "IX_archive_mother_id" ON public.archive USING btree (mother_id);
 *   DROP INDEX public."IX_archive_mother_id";
       public            postgres    false    215            �           1259    36394    IX_archive_owner_id    INDEX     M   CREATE INDEX "IX_archive_owner_id" ON public.archive USING btree (owner_id);
 )   DROP INDEX public."IX_archive_owner_id";
       public            postgres    false    215            �           1259    36395    IX_awards_id_dog    INDEX     G   CREATE INDEX "IX_awards_id_dog" ON public.awards USING btree (id_dog);
 &   DROP INDEX public."IX_awards_id_dog";
       public            postgres    false    219            �           1259    36396    IX_dogs_exhibition_id    INDEX     Q   CREATE INDEX "IX_dogs_exhibition_id" ON public.dogs USING btree (exhibition_id);
 +   DROP INDEX public."IX_dogs_exhibition_id";
       public            postgres    false    221            �           1259    36397    IX_dogs_father_id    INDEX     I   CREATE INDEX "IX_dogs_father_id" ON public.dogs USING btree (father_id);
 '   DROP INDEX public."IX_dogs_father_id";
       public            postgres    false    221            �           1259    36398    IX_dogs_mother_id    INDEX     I   CREATE INDEX "IX_dogs_mother_id" ON public.dogs USING btree (mother_id);
 '   DROP INDEX public."IX_dogs_mother_id";
       public            postgres    false    221            �           1259    36399    IX_dogs_owner_id    INDEX     G   CREATE INDEX "IX_dogs_owner_id" ON public.dogs USING btree (owner_id);
 &   DROP INDEX public."IX_dogs_owner_id";
       public            postgres    false    221            �           2620    36400    awards award_date_trigger    TRIGGER     �   CREATE TRIGGER award_date_trigger BEFORE INSERT OR UPDATE ON public.awards FOR EACH ROW EXECUTE FUNCTION public.award_date_check();
 2   DROP TRIGGER award_date_trigger ON public.awards;
       public          postgres    false    228    219            �           2620    36401    dogs check_breed_trigger    TRIGGER     s   CREATE TRIGGER check_breed_trigger AFTER INSERT ON public.dogs FOR EACH ROW EXECUTE FUNCTION public.check_breed();
 1   DROP TRIGGER check_breed_trigger ON public.dogs;
       public          postgres    false    221    229            �           2606    36402 !   archive FK_archive_dogs_father_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.archive
    ADD CONSTRAINT "FK_archive_dogs_father_id" FOREIGN KEY (father_id) REFERENCES public.dogs(id_dog);
 M   ALTER TABLE ONLY public.archive DROP CONSTRAINT "FK_archive_dogs_father_id";
       public          postgres    false    221    215    3236            �           2606    36407 !   archive FK_archive_dogs_mother_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.archive
    ADD CONSTRAINT "FK_archive_dogs_mother_id" FOREIGN KEY (mother_id) REFERENCES public.dogs(id_dog);
 M   ALTER TABLE ONLY public.archive DROP CONSTRAINT "FK_archive_dogs_mother_id";
       public          postgres    false    215    3236    221            �           2606    36412 +   archive FK_archive_exhibition_exhibition_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.archive
    ADD CONSTRAINT "FK_archive_exhibition_exhibition_id" FOREIGN KEY (exhibition_id) REFERENCES public.exhibition(id_exhibition);
 W   ALTER TABLE ONLY public.archive DROP CONSTRAINT "FK_archive_exhibition_exhibition_id";
       public          postgres    false    215    3238    223            �           2606    36417 "   archive FK_archive_owners_owner_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.archive
    ADD CONSTRAINT "FK_archive_owners_owner_id" FOREIGN KEY (owner_id) REFERENCES public.owners(owner_id) ON DELETE CASCADE;
 N   ALTER TABLE ONLY public.archive DROP CONSTRAINT "FK_archive_owners_owner_id";
       public          postgres    false    215    3240    225            �           2606    36422    awards FK_awards_dogs_id_dog    FK CONSTRAINT     �   ALTER TABLE ONLY public.awards
    ADD CONSTRAINT "FK_awards_dogs_id_dog" FOREIGN KEY (id_dog) REFERENCES public.dogs(id_dog) ON DELETE CASCADE;
 H   ALTER TABLE ONLY public.awards DROP CONSTRAINT "FK_awards_dogs_id_dog";
       public          postgres    false    219    3236    221            �           2606    36427    dogs FK_dogs_dogs_father_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.dogs
    ADD CONSTRAINT "FK_dogs_dogs_father_id" FOREIGN KEY (father_id) REFERENCES public.dogs(id_dog);
 G   ALTER TABLE ONLY public.dogs DROP CONSTRAINT "FK_dogs_dogs_father_id";
       public          postgres    false    221    221    3236            �           2606    36432    dogs FK_dogs_dogs_mother_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.dogs
    ADD CONSTRAINT "FK_dogs_dogs_mother_id" FOREIGN KEY (mother_id) REFERENCES public.dogs(id_dog);
 G   ALTER TABLE ONLY public.dogs DROP CONSTRAINT "FK_dogs_dogs_mother_id";
       public          postgres    false    221    221    3236            �           2606    36437 %   dogs FK_dogs_exhibition_exhibition_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.dogs
    ADD CONSTRAINT "FK_dogs_exhibition_exhibition_id" FOREIGN KEY (exhibition_id) REFERENCES public.exhibition(id_exhibition);
 Q   ALTER TABLE ONLY public.dogs DROP CONSTRAINT "FK_dogs_exhibition_exhibition_id";
       public          postgres    false    3238    223    221            �           2606    36442    dogs FK_dogs_owners_owner_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.dogs
    ADD CONSTRAINT "FK_dogs_owners_owner_id" FOREIGN KEY (owner_id) REFERENCES public.owners(owner_id) ON DELETE CASCADE;
 H   ALTER TABLE ONLY public.dogs DROP CONSTRAINT "FK_dogs_owners_owner_id";
       public          postgres    false    3240    221    225            B   �   x�e�;B1Dњ,&��|��D�DI��%��"�)R���nAز��x�_�ۨV��/ӑ!6Q��I��%5��Bib$u�e�4�~'���y!L͹7B��洚��M��gA���>�Ɲ -�8s'�^K)_Zh�      C   �   x�]���@D��ػY�g)�
(�{0m5��f;rAC���^2﯄��g�;��+�p�[�r�BC��;�t�y��ky�M�_����V������.�j�yÀ}f�����5�����XW�O�O�;[�4�Ƣ(��ωb��n2�Kc�AM+      E   !   x�3�LL��̃��\F��ũE+F��� �=	I      G   �   x���=�0�g��QQb7��,\�t �q.P�X�r�F4]���)[���g;�$~J%{9p-�HJ~�k~p��)�6�4�zӿ�&e����7���hIm]D&��0�� �@��0�=���)�!���Hf\u�A���!g򳵎�<s7�F�Rʮ�6N<���QC��`|D�Չ���p�CM-���Y�K~��a��������t۵R��AB�      I   j  x��T[NA��9���D��؇�	|J��(A�#R���8&��zn��޵���+lY�5U�U�^�<��>+P�Bel4�N���sd��+|Rt�W���-��Gz@{�?p���T@�8k,�9��t��t��p�N��
�a��3��ؒ�F0
�.�5Df�N��%�z����D���F&�:.K�O2���lI�W�Ji�0v�36�u��ȑ{�FD_6��K��l��F�y��{&
��+��ɳ�5���s����tݑ �N!"�CW5X�_��n6R
��Rȍ�pV��zC =Bo�r�&#�I�:��VP�ʔ8��N�<Z�	��ΰ!����X������r_����N�C�p^ ��zb�H� �(�� Z��j�{�j�N!��|^'�S������H��y�H&@���ݎ�%�(�n�s��/�n$�J�e_�������`�+��s��C��O"�w&�q-�\�A\�f )$ZM�v����A,��+9׊j��*�y����?lM�P۽���`���Sn�B�S"Uo|�޸�d/�ԅ;[m,�m|��T�X��5;֍�l�& Sp[/��R��K�m#H��<7{����ʮ      K     x�u�an�0�'���)S8�N�ì�:&mҴ�h+U��+�7��T�0&!���/6 @���iK/Xc�֊�8`�e����L{�?8�3�'a���8NSޮ���z*K+��^hO;��=�A�B[�gF�&���o&^�����% L_q�����<#.`��'-V�-�h��<q�1�{���`�QN��X-����0ero�m��Ms��$7Cn�\7�@�͓����y�@�;m�(p�K�ͦ�,�1���]��/{�QEo���ɟw��_��h�<J)mj�+      M   �  x��RIn�0]ۧ`�a�1ܥ�H�A.�AU+�꼨�!�PH� ��}��o
���@���V氶;�,�	�XB{����2;���`n��J��R��`�{����,ұ0�H1X��g�`��`�;&�-:$oȜC�Z��Hͤ�"�R\3�!���7���[!DAv2��.],\���qwf�j
4�i�EE����bS��K/��j�g�q t��4I"��a�_���S�kD�1���>��?G��1D�Elx��>�C�?�8�V5eeGvB�`o���H~卵28�^�
��CpfR�'*Hy���ċ �K�P�'!��}�����p�>���r������t���1Vk�Y0�L���()ɯ}��/�ֶj     
<script lang="ts">
	import { onMount } from 'svelte';
	import { type Message } from '$lib/message';
	import { LINKS } from '$lib/links';
	import { PHYSICS } from '$lib/physics';

	interface Material {
		link: string;
		physics: string;
	}

	interface Conversion {
		material: Material;
		newLink: string;
		newPhysics: string;
	}

	onMount(() => {
		window.external.receiveMessage((message: string) => {
			console.log('Received message: ' + message);
			handleMessage(message);
		});
	});

	let blocks_filelist: FileList | null = $state(null);
	let block_data: any = $state([]);
	let final_mats: Material[] = $state([]);
	let conversions: Conversion[] = $state([]);
	let converted_blocks: any[] = $state([]);

	function sendMessage(Message: Message) {
		console.log('We are in sendMessage');
		if (window === undefined || window.external === undefined) {
			console.error('Tried to send a message but the window.external object is not available');
			return;
		}
		Message.data = JSON.stringify(Message.data);
		console.log('Sending message: ' + JSON.stringify(Message));
		window.external.sendMessage(JSON.stringify(Message));
		console.log('Message sent');
	}

	function handleMessage(messageJson: string) {
		const message = JSON.parse(messageJson);
		const data = JSON.parse(message.data);
		console.log('Handling message: ' + JSON.stringify(message));
		switch (message.command) {
			case 'error':
				alert(data);
				break;
			case 'blocks_info':
				data.forEach((element: any) => {
					//remove duplicate materials
					let filtered_mats: Material[] = [];

					element.materials.forEach((mat: Material) => {
						mat.link = prettifyLink(mat.link);
						if (
							filtered_mats.find((m) => m.link === mat.link && m.physics === mat.physics) ===
							undefined
						) {
							filtered_mats.push(mat);
						}
					});
					element.materials = filtered_mats;
				});
				block_data = data;

				console.log('Blocks data: ' + JSON.stringify(block_data));
				calculate_mats();
				break;
			case 'converted_blocks':
				console.log('Converted blocks: ' + JSON.stringify(data));
				converted_blocks = data;
				break;
		}
	}

	function prettifyLink(link: string) {
		return link.replace('Stadium\\Media\\Material\\', '');
	}

	function reformLink(link: string) {
		return 'Stadium\\Media\\Material\\' + link;
	}

	function addConversion() {
		conversions.push({
			material: final_mats[conversions.length],
			newLink: LINKS[0],
			newPhysics: PHYSICS[0]
		});
	}

	function calculate_mats() {
		let mats: Material[] = [];
		for (let block of block_data) {
			for (let mat of block.materials) {
				if (mats.find((m) => m.link === mat.link && m.physics === mat.physics) === undefined) {
					mats.push(mat);
				}
			}
		}
		final_mats = mats;
		console.log('Final mats: ' + JSON.stringify(final_mats));
		conversions = [];
		addConversion();
	}

	function readAsDataURL(file: File) {
		return new Promise((resolve, reject) => {
			let fileReader = new FileReader();
			fileReader.onload = function () {
				return resolve({
					byteStream: String(fileReader.result).split(',')[1], // hack to remove the header of the base64 string
					filename: file.name
				});
			};
			fileReader.readAsDataURL(file);
		});
	}
	async function updateBlocks(blocks_filelist: FileList) {
		let blocks_stream = await Promise.all(Array.from(blocks_filelist).map(readAsDataURL));
		let message = {
			command: 'read_blocks_info',
			data: blocks_stream
		};
		sendMessage(message);
		converted_blocks = [];
	}

	async function sendConversions() {
		let blocks_stream = await Promise.all(
			Array.from(blocks_filelist as FileList).map(readAsDataURL)
		);
		let reformedConversions = await conversions.map((c) => {
			return {
				material: {
					link: reformLink(c.material.link),
					physics: c.material.physics
				},
				newLink: reformLink(c.newLink),
				newPhysics: c.newPhysics
			};
		});
		let message = {
			command: 'convert',
			data: {
				conversions: reformedConversions,
				blockData: blocks_stream
			}
		};
		sendMessage(message);
	}

	$effect(() => {
		if (blocks_filelist && (blocks_filelist as FileList).length > 0) {
			let blocks_list = blocks_filelist as FileList;
			console.log('Blocks: ' + blocks_list);
			updateBlocks(blocks_list);
		}
	});

	$inspect(conversions, () => {
		console.log('Conversions: ' + JSON.stringify(conversions));
	});
</script>

<main>
	<article>
		<header><h4>Select Blocks</h4></header>
		<label for="blocks">Choose one or multiples blocks to edit (.Block.Gbx)</label>
		<input type="file" id="blocks" accept=".Block.Gbx" bind:files={blocks_filelist} multiple />
	</article>
	{#if block_data.length > 0}
		<article>
			<header>
				<h4>Selected Blocks</h4>
			</header>
			{#each block_data as bd}
				<p><b>{bd.filename}</b></p>
				<div class="table-responsible">
					<table>
						<thead>
							<tr>
								<th>Material (Link)</th>
								<th>Physics</th>
							</tr>
						</thead>
						<tbody>
							{#each bd.materials as mat}
								<tr>
									<td>{mat.link}</td>
									<td>{mat.physics}</td>
								</tr>
							{/each}
						</tbody>
					</table>
				</div>
			{/each}
		</article>
		<article>
			<header>
				<h4>Convert</h4>
			</header>
			{#each conversions as c}
				<fieldset class="flex row">
					<div>
						<label for="material">Material</label>
						<select id="material" bind:value={c.material}>
							{#each final_mats as mat}
								<option value={mat}>{mat.link} - {mat.physics}</option>
							{/each}
						</select>
					</div>
					<div>
						<label for="newLink">New Link</label>
						<select id="newLink" bind:value={c.newLink}>
							{#each LINKS as link}
								<option value={link}>{link}</option>
							{/each}
						</select>
					</div>
					<div>
						<label for="newPhysics">New Physics</label>
						<select id="newPhysics" bind:value={c.newPhysics}>
							{#each PHYSICS as physics}
								<option value={physics}>{physics}</option>
							{/each}
						</select>
					</div>
					<div>
						<button class="danger" onclick={() => conversions.splice(conversions.indexOf(c), 1)}
							>-</button
						>
					</div>
				</fieldset>
			{/each}
			{#if conversions.length < final_mats.length}
				<button class="success" onclick={() => addConversion()}>+</button>
			{/if}
			{#if conversions.length > 0}
				<article>
					<button onclick={() => sendConversions()}>Convert</button>
				</article>
			{/if}
		</article>
	{/if}
	{#if converted_blocks.length > 0}
		<article>
			<header>
				<h4>Converted Blocks</h4>
			</header>
			{#each converted_blocks as cb}
				<p><b>{cb.filename}</b></p>
				<a download={cb.filename} href={'data:application/gbx;base64,' + cb.byteStream}>Download</a>
			{/each}
		</article>
	{/if}
</main>
